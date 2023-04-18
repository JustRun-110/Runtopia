package com.ssafy.api.request;

import com.fasterxml.jackson.annotation.JsonFormat;
import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;
import java.time.LocalTime;

@Getter
@Setter
@ApiModel("UserRankingReq") //Swagger와 함께 사용할 클래스에 대한 메타데이터 및 문서를 제공하는 데 사용됩니다.
public class UserRankingReq {
    @ApiModelProperty(name = "유저 ID", example = "user_id")
    String userId; // id 값

//    기존 유저레이팅 점수는 db에서 가져 옵니다.
//    @ApiModelProperty(name="userRating", example = "rating_Point")
//    Integer userRating; // 기존 유저 rating 값

    @ApiModelProperty(name = "유저 낙사 횟수", example = "user_drop")
    Integer userDrop; // 유저가 드랍한 횟수

    @ApiModelProperty(name = "유저가 먹은 코인 횟수", example = "user_Coin")
    Integer userCoin; // 유저가 먹은 코인 갯수

    @ApiModelProperty(name = "유저들의 플레이 타임", example = "user_PlayTime")
    @JsonFormat(shape = JsonFormat.Shape.STRING, pattern = "HH:mm:ss")
    LocalTime userPlayTime; // 유저 게임 플레이타임


    @ApiModelProperty(name = "유저가 이겼는지 졌는지", example = "user_win")
    boolean getWin; // 유저 게임 플레이타임


}
