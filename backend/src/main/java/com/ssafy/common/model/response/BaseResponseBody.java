package com.ssafy.common.model.response;

import org.springframework.http.HttpStatus;

import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;
import lombok.Getter;
import lombok.Setter;

// TODO
//  - 서버 요청에대한 기본 응답값(바디) 정의.
//  - PI 요청의 응답 본문을 나타내는 "BaseResponseBody"라는 Java 클래스입니다.
//  - 여기에는 "message" 및 "statusCode"라는 두 가지 속성이 있으며 둘 다 null로 초기화됩니다.
//  - 기본 생성자, 상태 코드를 받는 생성자, 상태 코드와 메시지를 모두 받는 생성자 3가지가 있음.
//  - "of"정적 메서드를 통해서 지정된 상태 코드 및 메시지가 있는 클래스의 새 인스턴스를 만들고 반환한다.
@Getter
@Setter
@ApiModel("BaseResponseBody")
public class BaseResponseBody {
	@ApiModelProperty(name="응답 메시지", example = "정상")
	String message = null;
	@ApiModelProperty(name="응답 코드", example = "200")
	Integer statusCode = null;
	
	public BaseResponseBody() {}
	
	public BaseResponseBody(Integer statusCode){
		this.statusCode = statusCode;
	}
	
	public BaseResponseBody(Integer statusCode, String message){
		this.statusCode = statusCode;
		this.message = message;
	}
	
	public static BaseResponseBody of(Integer statusCode, String message) {
		BaseResponseBody body = new BaseResponseBody();
		body.message = message;
		body.statusCode = statusCode;
		return body;
	}
}
