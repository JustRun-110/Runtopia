package com.ssafy.api.service;

import com.ssafy.api.request.UserRankingReq;
import com.ssafy.api.response.UserMmrRanking;
import com.ssafy.db.entity.Achieve;

import java.util.List;

/**
 * ELO 등급 계산기
 * <p>
 * 두 가지 방법 구현
 * 1. calculateMultiplayer - 여러 플레이어의 등급을 계산한다.
 * 2. calculate2PlayerRating - 2인용 게임용
 */
public interface UserRankingService {
    Achieve calculateMultiPlayer(UserRankingReq userRankingReq);

    List<UserMmrRanking> getAllUserMmrRankings();
}
