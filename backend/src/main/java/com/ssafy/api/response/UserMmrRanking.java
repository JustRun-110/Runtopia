package com.ssafy.api.response;

import io.swagger.annotations.ApiModel;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@ApiModel("UserMMRRanking")
public class UserMmrRanking {
    private String userId;
    private Long mmr;
    private int overallRanking;

    public UserMmrRanking(String userId, Long mmr, int overallRanking) {
        this.userId = userId;
        this.mmr = mmr;
        this.overallRanking = overallRanking;
    }
}
