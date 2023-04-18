package com.ssafy.api.controller;

import com.ssafy.api.request.UserUpdatePostReq;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.*;
import com.ssafy.api.request.UserLoginPostReq;
import com.ssafy.api.request.UserRegisterPostReq;
import com.ssafy.api.response.UserRes;
import com.ssafy.api.service.UserService;
import com.ssafy.common.model.response.BaseResponseBody;
import com.ssafy.db.entity.User;

import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import io.swagger.annotations.ApiParam;
import io.swagger.annotations.ApiResponse;
import io.swagger.annotations.ApiResponses;

/**
 * 유저 관련 API 요청 처리를 위한 컨트롤러 정의.
 */
@Api(value = "유저 API", tags = {"User"})
@RestController
@RequestMapping("/api/v1/users")
public class UserController {

    @Autowired
    UserService userService;

    @Autowired
    PasswordEncoder passwordEncoder;

    @PostMapping()
    @ApiOperation(value = "회원 가입", notes = "<strong>아이디와 패스워드</strong>를 통해 회원가입 한다.")
    @ApiResponses({
            @ApiResponse(code = 200, message = "성공"),
            @ApiResponse(code = 401, message = "인증 실패"),
            @ApiResponse(code = 404, message = "사용자 없음"),
            @ApiResponse(code = 500, message = "서버 오류")
    })
    public ResponseEntity<? extends BaseResponseBody> register(
            @RequestBody @ApiParam(value = "회원가입 정보", required = true) UserRegisterPostReq registerInfo) {

        //임의로 리턴된 User 인스턴스. 현재 코드는 회원 가입 성공 여부만 판단하기 때문에 굳이 Insert 된 유저 정보를 응답하지 않음.
        User user = userService.createUser(registerInfo);

        return ResponseEntity.status(200).body(BaseResponseBody.of(200, "Success"));
    }


    @GetMapping("/me/{userId}")
    @ApiOperation(value = "회원 본인 정보 조회", notes = "로그인한 회원 본인의 정보를 응답한다.")
    @ApiResponses({
            @ApiResponse(code = 200, message = "성공"),
            @ApiResponse(code = 401, message = "인증 실패"),
            @ApiResponse(code = 404, message = "사용자 없음"),
            @ApiResponse(code = 500, message = "서버 오류")
    })
    public ResponseEntity<? extends UserRes> getUser(
            @PathVariable @ApiParam(value = "User ID", required = true) String userId) {

        User user = userService.getUserById(userId);

        // todo
        //  예외처리 (*.*)

        UserRes userRes = UserRes.of(user);
        return ResponseEntity.ok(userRes);

    }

    @PostMapping("/login")
    @ApiOperation(value = "회원으로 로그인", notes = "<strong>아이디와 비밀번호</strong>로 로그인하세요.")
    @ApiResponses({
            @ApiResponse(code = 200, message = "성공"),
            @ApiResponse(code = 401, message = "인증 실패"),
            @ApiResponse(code = 404, message = "사용자 없음"),
            @ApiResponse(code = 500, message = "서버 오류")
    })
    public ResponseEntity<? extends BaseResponseBody> login(
            @RequestBody @ApiParam(value = "Login information", required = true) UserLoginPostReq loginInfo) {

        String userId = loginInfo.getUserId();
        String password = loginInfo.getPassword();
        User user = userService.getUserById(userId);

        //로그인 요청한 유저로부터 입력된 패스워드 와 디비에 저장된 유저의 암호화된 패스워드가 같은지 확인.(유효한 패스워드인지 여부 확인)
        if(passwordEncoder.matches(password, user.getPassword())){
            return ResponseEntity.status(200).body(BaseResponseBody.of(200, "Success"));
        }
        else {
            return ResponseEntity.status(401).body(BaseResponseBody.of(401, "Authentication failed"));
        }
    }


    @PatchMapping("/{userId}")
    @ApiOperation(value = "Modify member information", notes = "Update member information with new values.")
    @ApiResponses({
            @ApiResponse(code = 200, message = "Success"),
            @ApiResponse(code = 400, message = "Bad Request"),
            @ApiResponse(code = 401, message = "Authentication failed"),
            @ApiResponse(code = 404, message = "No user"),
            @ApiResponse(code = 500, message = "Server Error")
    })
    public ResponseEntity<? extends UserRes> updateUserX(
            @PathVariable @ApiParam(value = "User ID", required = true) String userId,
            @RequestBody @ApiParam(value = "User Information", required = true) UserUpdatePostReq updateInfo) {

        User user = userService.getUserById(userId);

        if (user == null) {
            // 유저가 없으면, return a 404 error response
            return ResponseEntity.notFound().build();
        }

        user.setNickname(updateInfo.getNickname());
        user.setPassword(updateInfo.getPassword());
        user.setGender(updateInfo.getGender());
        //user.setUserId(updateInfo.getUserId());

        // Save the updated user information
        User updatedUser = userService.updateUser(user);

        // Return a success response with the updated user information
        return ResponseEntity.ok(UserRes.of(updatedUser));
    }


    @DeleteMapping("/{userId}")
    @ApiOperation(value = "회원 탈퇴", notes = "회원 본인의 정보를 삭제한다.")
    @ApiResponses({
            @ApiResponse(code = 200, message = "성공"),
            @ApiResponse(code = 401, message = "인증 실패"),
            @ApiResponse(code = 404, message = "사용자 없음"),
            @ApiResponse(code = 500, message = "서버 오류")
    })
    public ResponseEntity<? extends BaseResponseBody> withdraw(
            @PathVariable @ApiParam(value = "User ID", required = true) String userId) {

        userService.deleteUserById(userId);

        return ResponseEntity.status(200).body(BaseResponseBody.of(200, "Success"));
    }

    @GetMapping("/check-nickname/{nickname}")
    @ApiOperation(value = "닉네임 유효성 검사", notes = "등록에 있어서 유효한 닉네임인지 체크를 하는 메서드이다.")
    @ApiResponses({
            @ApiResponse(code = 200, message = "Success"),
            @ApiResponse(code = 500, message = "Server Error")
    })
    public ResponseEntity<? extends BaseResponseBody> checkNicknameAvailability(
            @PathVariable @ApiParam(value = "Nickname", required = true) String nickname) {

        boolean isNicknameAvailable = userService.isNicknameAvailable(nickname);

        if (isNicknameAvailable) {
            // 만약 닉네임이 사용가능하면 200 ok
            return ResponseEntity.status(200).body(BaseResponseBody.of(200, "Success"));
        } else {
            // 만약 닉네임이 이미 있다면 409 return
            return ResponseEntity.status(409).body(BaseResponseBody.of(409, "Nickname already exists"));
        }
    }
}



