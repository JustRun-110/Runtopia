package com.ssafy.api.response;

import com.ssafy.common.model.response.BaseResponseBody;
import com.ssafy.db.entity.User;

import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;
import lombok.Getter;
import lombok.Setter;

/**
 * 회원 본인 정보 조회 API ([GET] /api/v1/users/me) 요청에 대한 응답값 정의.
 */
@Getter
@Setter
@ApiModel("UserResponse")
public class UserRes {
    @ApiModelProperty(name = "User ID")
    String userId;
    @ApiModelProperty(name = "User nickname")
    String nickname;
    @ApiModelProperty(name = "User gender")
    String gender;


    // User 객체에 포함된 정보를 기반으로 새로운 UserRes 객체를 생성하는 정적 팩토리 메서드입니다.
    public static UserRes of(User user) {
        UserRes res = new UserRes();
        res.setUserId(user.getUserId());
        res.setNickname(user.getNickname());
        res.setGender(String.valueOf(user.getGender()));
        return res;
    }


}
