package com.ssafy.common.util;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.Calendar;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.http.HttpStatus;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.common.collect.ImmutableMap;
import com.ssafy.common.model.response.BaseResponseBody;

import static org.springframework.http.MediaType.APPLICATION_JSON_VALUE;

/**
 *
 */
// todo
//  API 응답 및 오류 메시지를 HTTP 응답에 쓰기 위한 유틸리티 메서드를 제공하는 클래스이다.
//  컨트롤러(controller)가 아닌곳에서, 서버 응답값(바디)을 직접 변경 및 전달 하기위한 유틸 정의.


public class ResponseBodyWriteUtil {

    // HttpServletResponse 객체와 API 응답을 나타내는 BaseResponseBody 객체를 받습니다. HTTP 상태 코드를 200 OK로 설정하고,
    // 콘텐츠 유형을 "application/json"으로 설정하고, BaseResponseBody 개체의 JSON 표현을 응답의 출력 스트림에 씁니다.
    public static void sendApiResponse(HttpServletResponse response, BaseResponseBody apiResponse) throws IOException {
        response.setStatus(HttpStatus.OK.value());
        response.setContentType(APPLICATION_JSON_VALUE);
        response.setCharacterEncoding("UTF-8");
        response.getOutputStream().write(new ObjectMapper().writeValueAsString(apiResponse).getBytes());
    }

    public static void sendError(HttpServletRequest request, HttpServletResponse response, Exception ex, HttpStatus httpStatus) throws IOException {
        response.setStatus(httpStatus.value());
        response.setContentType(APPLICATION_JSON_VALUE);
        response.setCharacterEncoding("UTF-8");
        String message = ex.getMessage();
        message = message == null ? "" : message;
        Map<String, Object> data = ImmutableMap.of(
                "timestamp", Calendar.getInstance().getTime(),
                "status", httpStatus.value(),
                "error", ex.getClass().getSimpleName(),
                "message", message,
                "path", request.getRequestURI()
        );
        PrintWriter pw = response.getWriter();
        pw.print(new ObjectMapper().writeValueAsString(data));
        pw.flush();
    }

    public static void sendError(HttpServletRequest request, HttpServletResponse response, Exception ex) throws IOException {
        sendError(request, response, ex, HttpStatus.UNAUTHORIZED);
    }
}
