package com.ssafy.api.service;

import com.ssafy.api.request.UserRankingReq;
import com.ssafy.api.response.UserMmrRanking;
import com.ssafy.db.entity.Achieve;
import com.ssafy.db.entity.User;
import com.ssafy.db.repository.RankingRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;

import java.time.Duration;
import java.time.LocalTime;
import java.util.ArrayList;
import java.util.List;

@Service("userRankingService")
public class UserRankingServiceImpl implements UserRankingService {
    @Autowired
    private RankingRepository rankingRepository;

    // TODO 메인 MMR 계산 메서드
    @Override
    public Achieve calculateMultiPlayer(UserRankingReq userRankingReq) {
        // 사용자의 업적 정보를 가져온다.
        Achieve achieve = rankingRepository.findByUserId(userRankingReq.getUserId());

        // 유저 업적 정보에 없는 경우 (null일 경우)
        if (achieve == null) {
            // 새로운 업적과 user ID를 설정하여 생성한다.
            achieve = new Achieve();
            achieve.setUserId(userRankingReq.getUserId());

            achieve.setMmr(1000L); // 초기 MMR을 1000으로 설정한다.
            achieve.setCoinCount(0L); // 초기 코인의 카운트 값을 0으로 설정한다.

            achieve.setWinCount(0L);

            // playTime과 bestTime을 0 hours, 0 minutes, 0 seconds로 초기화
            achieve.setPlayTime(LocalTime.of(0, 0, 0));
            achieve.setBestTime(LocalTime.of(0, 0, 0));
        }

        // 요청된 매개변수들을 기반으로 새 사용자 평점을 계산한다.
        long newRating = calculateNewUserRating(achieve, userRankingReq);

        // 새로운 평점과 다른 관련 정보로 Achieve 기록을 업데이트 한다.
        achieve.setMmr(newRating);
        achieve.setCoinCount(achieve.getCoinCount() + userRankingReq.getUserCoin());

        LocalTime updatedPlayTime = addPlayDurations(achieve.getPlayTime(), userRankingReq.getUserPlayTime());
        achieve.setPlayTime(updatedPlayTime);

        String tier = calculateTier(newRating);
        achieve.setTier(tier);

        if (userRankingReq.isGetWin() == true) {
            achieve.setBestTime(userRankingReq.getUserPlayTime());
            achieve.setWinCount(achieve.getWinCount() + 1);
        }

        // 달성 기록에서 플레이 시간 업데이트
        achieve.setPlayTime(updatedPlayTime);

        // 업데이트 된 Achieve 기록을 저장한다.
        rankingRepository.save(achieve);

        // 업데이트 된 User객체를 반환한다.
        // User 객체 구현에 따라 이부분은 조정이 될 수 있다.
        User user = new User();
        user.setUserId(userRankingReq.getUserId());
        user.setMmr(achieve.getMmr());

        return achieve;
    }

    // TODO 랭킹 보드에 뿌려줄 데이터
    @Override
    public List<UserMmrRanking> getAllUserMmrRankings() {
        // Achieve를 검색해서, sorted by MMR 내림차순
        List<Achieve> achieveList = rankingRepository.findAll(Sort.by(Sort.Direction.DESC, "mmr"));

        List<UserMmrRanking> mmrRankings = new ArrayList<>();
        int rank = 1;

        for (Achieve achieve : achieveList) {
            UserMmrRanking userMmrRanking = new UserMmrRanking(achieve.getUserId(), achieve.getMmr(), rank);
            mmrRankings.add(userMmrRanking);
            rank++;
        }

        return mmrRankings;
    }

    // TODO 새로운 유저의 MMR을 계산한다.
    private long calculateNewUserRating(Achieve achieve, UserRankingReq userRankingReq) {
        // 상수
        final int DROP_PENALTY = 10;
        final int COIN_BONUS = 5;
        final int LOSS_PENALTY = 10;

        // 기존 사용자 평점 가져오기
        Long currentRating = achieve.getMmr();

        // 낙하 페널티 적용
        currentRating -= userRankingReq.getUserDrop() * DROP_PENALTY;

        // 코인 보너스 적용
        currentRating += userRankingReq.getUserCoin() * COIN_BONUS;

        // 사용자와 상대방(들)에 대한 예상 점수 계산
        // 간단하게 평균 상대 평점을 가정합니다
        int avgOpponentRating = 1500; // 게임의 평균 사용자 평점을 기반으로 이 값을 조정하세요
        double expectedScore = 1 / (1 + Math.pow(10, (avgOpponentRating - currentRating) / 400.0));

        // 게임 결과에 기반한 실제 점수 계산
        // 승리에 대한 이진 결과를 가정: 승리에 대해 1, 패배에 대해 0
        // 게임 메커니즘에 따라 이를 조정할 수 있습니다
        int actualScore = userRankingReq.isGetWin() ? 1 : 0;

        long ratingDifference = Math.abs(currentRating - avgOpponentRating);

        // 레이팅 차등에 따른 K-factor 조정
        int kFactor;
        if (ratingDifference < 200) {
            kFactor = 32;
        } else if (ratingDifference < 400) {
            kFactor = 24;
        } else {
            kFactor = 16;
        }

        // ELO 수식을 사용하여 새 평점 계산
        long newRating = currentRating + (int) (kFactor * (actualScore - expectedScore));

        // 진 유저에게 mmr 적용
        if (!userRankingReq.isGetWin()) {
            newRating -= LOSS_PENALTY;
        }
        return newRating;
    }

    //TODO 기존 플레이 시간과 새 플레이 시간 사이의 기간 계산
    private LocalTime addPlayDurations(LocalTime existingPlayTime, LocalTime newPlayTime) {
        Duration newPlayDuration = Duration.between(LocalTime.MIN, newPlayTime);
        return existingPlayTime.plus(newPlayDuration);
    }

    //TODO user tier를 계산
    private String calculateTier(long mmr) {
        if (mmr >= 2400) {
            return "Grandmaster";
        } else if (mmr >= 2000) {
            return "Master";
        } else if (mmr >= 1600) {
            return "Diamond";
        } else if (mmr >= 1200) {
            return "Platinum";
        } else if (mmr >= 800) {
            return "Gold";
        } else if (mmr >= 400) {
            return "Silver";
        } else {
            return "Bronze";
        }
    }
}
