package com.ssafy.api.controller;

import com.ssafy.api.request.UserRankingReq;
import com.ssafy.api.response.RankingRes;
import com.ssafy.api.response.UserMmrRanking;
import com.ssafy.api.service.UserRankingService;
import com.ssafy.api.service.UserService;
import com.ssafy.common.model.response.BaseResponseBody;
import com.ssafy.db.entity.Achieve;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import io.swagger.annotations.ApiParam;
import io.swagger.annotations.ApiResponse;
import io.swagger.annotations.ApiResponses;

import java.util.List;

@Api(value = "랭킹 API", tags = {"Ranking"})
@RestController
@RequestMapping("/api/v1/ranking")
public class RankingController {

    @Autowired
    UserService userService;

    @Autowired
    UserRankingService userRankingService;

    @PostMapping()
    @ApiOperation(value = "랭킹 정보 갱신", notes = "랭킹 정보를 받아서 계산 후에 갱신한다.")
    @ApiResponses({
            @ApiResponse(code = 200, message = "성공"),
            @ApiResponse(code = 401, message = "인증 실패"),
            @ApiResponse(code = 404, message = "사용자 없음"),
            @ApiResponse(code = 500, message = "서버 오류")
    })
    public ResponseEntity<? extends BaseResponseBody> refreshRanking
            (@RequestBody @ApiParam(value = "유저 랭킹시스템 관리", required = true) UserRankingReq userRankingReq) {

        Achieve achieve = userRankingService.calculateMultiPlayer(userRankingReq);
        return ResponseEntity.status(200).body(RankingRes.of(200, "Success", achieve));

    }

    @GetMapping()
    @ApiOperation(value = "mmr랭킹을 조회 한다.", notes = "모든 사용자의 MMR 순위를 검색합니다.")
    @ApiResponses({
            @ApiResponse(code = 200, message = "Success"),
            @ApiResponse(code = 401, message = "Authentication failed"),
            @ApiResponse(code = 500, message = "Server Error")
    })
    public ResponseEntity<List<UserMmrRanking>> getMmrRankings() {
        List<UserMmrRanking> mmrRankings = userRankingService.getAllUserMmrRankings();
        return ResponseEntity.status(200).body(mmrRankings);
    }


}
