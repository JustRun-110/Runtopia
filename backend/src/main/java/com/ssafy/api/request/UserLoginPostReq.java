package com.ssafy.api.request;


import com.ssafy.db.entity.User;
import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;

/**
 * 유저 로그인 API ([POST] /api/v1/auth/login) 요청에 필요한 리퀘스트 바디 정의.
 */
@Getter
@Setter
@ApiModel("UserLoginPostRequest") //Swagger와 함께 사용할 클래스에 대한 메타데이터 및 문서를 제공하는 데 사용됩니다.
public class UserLoginPostReq {
	@ApiModelProperty(name="유저 ID", example="your_id") //sweager값 사용
	String userId; // id 값
	@ApiModelProperty(name="유저 Password", example="your_password")
	String password; // password 값
	@ApiModelProperty(name="유저 nickname", example="your_nickname")
	String nickname;
	@ApiModelProperty(name="유저 gender", example="your_gender")
	Boolean gender;

	public static UserLoginPostReq toUserLoginPostReq(User user){
		UserLoginPostReq userLoginPostReq = new UserLoginPostReq();

		userLoginPostReq.setUserId(user.getUserId());
		userLoginPostReq.setPassword(user.getPassword());
		userLoginPostReq.setGender(user.getGender());
		userLoginPostReq.setNickname(user.getNickname());
		return userLoginPostReq;
	}
}
