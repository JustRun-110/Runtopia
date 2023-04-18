package com.ssafy.handler;

import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.RedisCallback;
import org.springframework.data.redis.core.RedisOperations;
import org.springframework.data.redis.core.SessionCallback;
import org.springframework.data.redis.core.StringRedisTemplate;
import org.springframework.web.socket.TextMessage;
import org.springframework.web.socket.WebSocketSession;
import org.springframework.web.socket.handler.TextWebSocketHandler;
import org.springframework.web.socket.CloseStatus;

import java.io.IOException;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

@Slf4j
public class MyWebSocketHandler extends TextWebSocketHandler {

    private static final int MAX_CONNECTIONS = 20;

    @Autowired
    private StringRedisTemplate stringRedisTemplate;

    private final Map<String, WebSocketSession> activeSessions = new ConcurrentHashMap<>();


    // TODO WebSocket 클라이언트 연결이 설정 된 후 실행되는 메서드
    @Override
    public void afterConnectionEstablished(WebSocketSession session) {
        //flushAll();
        System.out.println("afterConnectionEstablished called!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        // 연결된 클라인언트의 세션 ID를 출력
        System.out.println("New connection established: " + session.getId());

        // 세션목록에 추가
        activeSessions.put(session.getId(), session);

        // 현재 연결된 클라이언트 수를 가져와서 currentConnections 변수에 저장한다.
        int currentConnections = getCurrentConnections();
        System.out.println("현재 접속중인 인원 : " + currentConnections);

        // 현재 연결된 수가 최대 허용 연결을 초과 했는지 확인한다.
        if (currentConnections >= MAX_CONNECTIONS) {
            // Queue에 추가
            addToQueue(session);
        } else {
            // 연결 수 증가
            // Websocket 서버에 대한 클라이언트 일반 엑세스 권한을 부여한다.
            processNormalAccess(session);
        }
    }

    public void afterConnectionClosed(WebSocketSession session, CloseStatus status) {
        System.out.println("Closed");
        String userId = session.getId();
        boolean isUserInQueue = isUserInQueue(userId);
        activeSessions.remove(userId);



        if (isUserInQueue) {
            // 대기열에 있는 사용자의 경우 처리
            stringRedisTemplate.opsForList().remove("waiting-queue", 1, userId);
        } else {
            // 게임 접속 중이던 사용자의 경우 처리
            stringRedisTemplate.opsForValue().decrement("current-connections");
            processNextInQueue();
        }
    }

    private boolean removeFromQueueIfPresent(WebSocketSession session) {
        long removedCount = stringRedisTemplate.opsForList().remove("waiting-queue", 1, session.getId());
        return removedCount > 0;
    }

    private WebSocketSession getNextInQueue() {
        String nextSessionId = stringRedisTemplate.opsForList().index("waiting-queue", 0);
        if (nextSessionId != null) {
            return activeSessions.get(nextSessionId);
        }
        return null;
    }

    private void updateWaitingInfoForAll() {
        List<String> waitingSessionIds = stringRedisTemplate.opsForList().range("waiting-queue", 0, -1);
        if (waitingSessionIds != null) {
            for (String sessionId : waitingSessionIds) {
                WebSocketSession waitingSession = activeSessions.get(sessionId);
                if (waitingSession != null) {
                    sendWaitingInfo(waitingSession);
                }
            }
        }
    }

    private void processNextInQueue() {
        WebSocketSession nextInQueue = getNextInQueue();
        if (nextInQueue != null) {
            processNormalAccess(nextInQueue);
            removeFromQueue(nextInQueue);
            updateWaitingInfoForAll();
        }
    }
    // TODO 유저가 최대 연결 수(20)에 도달하면 대기 대기열에 사용자를 추가합니다.
    private void addToQueue(WebSocketSession session) {
        // redistemplate를 사용하여 목록 끝에 세션 ID를 추가한다.
        // 클라이언트의 세션 ID가 Redis 스토어의 대기열에 추가된다.
        stringRedisTemplate.opsForList().rightPush("waiting-queue", session.getId());
        // 클라이언트에게 대기열에 추가 되었음을 알린다.
        // 대기열에서의 위치 및 가능한 예상 대기 시간에 대한 추가 정보를 클라이언트에 보낸다.
        sendWaitingInfo(session);
    }

    // TODO 일반적인 접근에 대한 처리 -> 사용자가 가득 안차 있을 때
    private void processNormalAccess(WebSocketSession session) {
        int currentConnections = getCurrentConnections();
        if (currentConnections < MAX_CONNECTIONS) {
            stringRedisTemplate.opsForValue().increment("current-connections");
            System.out.println("추가됨 : " + getCurrentConnections());
            activeSessions.put(session.getId(), session);

            sendMessage(session, "ok");

            initializeUser(session);
        } else {
            addToQueue(session);
        }
    }

    // TODO WebSocketSession에 텍스트 메시지를 보내는 데 사용되는 메서드
    private void sendMessage(WebSocketSession session, String message) {
        try {
            session.sendMessage(new TextMessage(message));
        } catch (IOException e) {
            System.out.println("Error sending message: " + e.getMessage());
        }
    }

    // TODO WebSocketSession에 연결할 때 새 사용자를 초기화하는 메서드
    //  사용자별 데이터를 데이터 저장소에 추가하거나 새 사용자 개체 만들기,
    //  사용자 초기 상태 설정 또는 특정 채널/주제에 대한 사용자 구독과 같은 사용자별 설정을 수행합니다.
    private void initializeUser(WebSocketSession session) {
        // 세션 ID를 사용자 식별자로 사용하여 새 사용자 개체를 만듭니다
        String sessionId = session.getId();

        // (사용자 클래스가 있다고 가정). User newUser = new User(session.getId());.
        //User newUser = new User(session.getId());

        // 지도, 데이터베이스 또는 메모리 내 데이터 구조와 같은 데이터 저장소에 사용자 개체를 저장합니다. 이 경우 사용자 개체는 사용자 지정 메서드
        // saveUserToRedis: saveUserToRedis(newUser);를 사용하여 Redis 데이터 저장소에 저장됩니다.
        saveUserToRedis(sessionId);

        // 사용자 정의 sendMessage 메소드를 사용하여 역할 및 구독 채널에 대해 사용자에게 알립니다.
    }

    // TODO 사용자 개체를 Redis 데이터 저장소에 저장하는 메서드
    private void saveUserToRedis(String sessionId) {
        // 사용자 ID와 연결하여 사용자에 대한 고유한 Redis 키를 생성한다. String userKey = "user:" + user.getId();.
        String userKey = "user:" + sessionId;

        //Redis에서 키-값 쌍으로 저장하기 위해 해시 작업에서 put 메서드 사용
        stringRedisTemplate.opsForHash().put(userKey, "id", sessionId);

        // 로깅 추가
        log.info("User saved to Redis: {}", stringRedisTemplate.opsForHash().entries(userKey));
    }

    // TODO 현재 연결된 수를 확인한다.

    private int getCurrentConnections() {
        String value = stringRedisTemplate.opsForValue().get("current-connections");
        return (value == null) ? 0 : Integer.parseInt(value);
    }

    // TODO WebSocketSession에 텍스트 메시지를 보내고 발생할 수 있는 모든 IOException을 처리합니다.
    //  IOException이 발생하면 대기열에서 세션을 제거하고 필요한 경우 추가 오류 처리를 허용합니다.
    private void notifyClient(WebSocketSession session, String message) {
        // PARAMETER : 텍스트 메시지를 보낼 WebSocket 세션과 Text message
        try {
            TextMessage response = new TextMessage(message);
            session.sendMessage(response);
        } catch (IOException e) {
            removeFromQueue(session);
        }
    }


    // TODO Redis 목록에서 WebSocketSession을 제거하는 메서드
    private void removeFromQueue(WebSocketSession session) {
        // "waiting-queue"는 WebSocket 세션이 저장된 Redis 목록의 키
        // 두 번째 매개변수 1은 제거할 요소의 발생 횟수를 지정합니다(이 경우 첫 번째 발생만 제거됨).
        // 세 번째 매개변수 session.getId()는 목록에서 제거할 WebSocket 세션의 ID입니다.
        stringRedisTemplate.opsForList().remove("waiting-queue", 1, session.getId());
    }

    // TODO WebSocketSession에 대기 정보를 보내는 메서드
    //      클라이언트에게 대기 중인 총 인원 수, 대기열에서의 위치 및 예상 대기 시간을 제공
    private void sendWaitingInfo(WebSocketSession session) {
        // stringRedisTemplate.opsForList()에서 size 메서드를 호출하여 "waiting-queue"라는 Redis 목록의 길이를 가져옵니다
        long queueLength = stringRedisTemplate.opsForList().size("waiting-queue");
        // stringRedisTemplate.opsForList()에서 indexOf 메서드를 호출하여 "waiting-queue" 목록에서 지정된 세션의 위치를 가져옵니다
        long position = stringRedisTemplate.opsForList().indexOf("waiting-queue", session.getId());
        // 대기열의 위치에 사용자당 예상 평균 대기 시간(이 경우 60초)을 곱하여 예상 대기 시간을 계산합니다.
        long estimatedWaitTime = position * 60;
        // 대기열 길이, 대기열 내 위치 및 예상 대기 시간이 포함된 형식화된 메시지를 전송하여 클라이언트에게 알립니다.
        notifyClient(session, String.format("fail_%d_%d_%d", queueLength, position, estimatedWaitTime));
    }

    // TODO WebSocket 연결을 통해 클라이언트로부터 받은 문자 메시지를 처리하는 메서드이다.

    @Override
    protected void handleTextMessage(WebSocketSession session, TextMessage message) {
        // 클라이언트로부터 받은 텍스트 메시지를 문자열로 변환
        String payload = message.getPayload();

        try {
            // 클라이언트와의 연결에 대한 세션 ID를 가져옴
            String sessionId = session.getId();

            // 문자열을 JSON 객체로 변환 (ObjectMapper 클래스를 사용)
            ObjectMapper objectMapper = new ObjectMapper();
            Map<String, Object> jsonMessage = objectMapper.readValue(payload, Map.class);

            // JSON 객체에서 "type" 키의 값을 가져옴
            String messageType = (String) jsonMessage.get("type");

            // 메시지 유형에 따라 처리 로직을 실행
            switch (messageType) {
                case "message":
                    String content = (String) jsonMessage.get("content");
                    // 메시지 처리 로직을 구현 (예: 메시지 내용 출력)
                    System.out.println("Received message: " + content);
                    break;
                // 다른 메시지 유형에 대한 처리 로직을 추가
                default:
                    System.out.println("Unknown message type: " + messageType);
                    break;
            }
        } catch (IOException e) {
            System.out.println("Error parsing message: " + e.getMessage());
        }
    }

    private boolean isUserInQueue(String id) {
        Long position = stringRedisTemplate.opsForList().indexOf("waiting-queue", id);
        return position != null && position >= 0;
    }

    public void flushAll() {
        stringRedisTemplate.execute((RedisCallback<Object>) connection -> {
            connection.flushAll();
            return "OK";
        });
    }
}