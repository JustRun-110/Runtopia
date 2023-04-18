package com.ssafy.api.response;

import com.ssafy.common.model.response.BaseResponseBody;
import com.ssafy.db.entity.Achieve;
import io.swagger.annotations.ApiModel;
import lombok.Getter;
import lombok.Setter;

/**
 * 유저 로그인 API ([POST] /api/v1/auth) 요청에 대한 응답값 정의.
 */
@Getter
@Setter
@ApiModel("RankingResponse")
public class RankingRes extends BaseResponseBody {
    private Achieve achieve;
    public static RankingRes of(Integer statusCode, String message,Achieve achieve) {
        RankingRes res = new RankingRes();
        res.setStatusCode(statusCode);
        res.setMessage(message);
        res.setAchieve(achieve);
        //res.setAccessToken(accessToken);
        return res;
    }
}
