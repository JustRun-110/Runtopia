package com.ssafy.api.service;

import com.ssafy.api.request.UserRegisterPostReq;
import com.ssafy.db.entity.User;

/**
 * 유저 관련 비즈니스 로직 처리를 위한 서비스 인터페이스 정의.
 */
public interface UserService {
    User createUser(UserRegisterPostReq userRegisterInfo);

    User getUserById(String userId);

    boolean authenticateUser(String userId, String password);

    User updateUser(User user);

    void deleteUserById(String userId);

    boolean isNicknameAvailable(String nickname);
}
