package com.ssafy.api.request;

import com.ssafy.db.entity.User;
import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;
import lombok.Getter;
import lombok.Setter;

/**
 * 유저 회원가입 API ([POST] /api/v1/users) 요청에 필요한 리퀘스트 바디 정의.
 */
@Getter
@Setter
@ApiModel("UserRegisterPostRequest") //Swagger와 함께 사용할 클래스에 대한 메타데이터 및 문서를 제공하는 데 사용됩니다.
public class UserRegisterPostReq {

    @ApiModelProperty(name = "유저 ID", example = "your_id") //sweager값 사용
    String userId; // id 값
    @ApiModelProperty(name = "유저 Password", example = "your_password")
    String password; // password 값
    @ApiModelProperty(name = "유저 nickname", example = "your_nickname")
    String nickname;
    @ApiModelProperty(name = "유저 gender", example = "your_gender")
    Boolean gender;

    public static UserRegisterPostReq toUserRegisterPostReq(User user) {
        UserRegisterPostReq userRegisterPostReq = new UserRegisterPostReq();

        userRegisterPostReq.setUserId(user.getUserId());
        userRegisterPostReq.setPassword(user.getPassword());
        userRegisterPostReq.setGender(user.getGender());
        userRegisterPostReq.setNickname(user.getNickname());
        return userRegisterPostReq;
    }

}
