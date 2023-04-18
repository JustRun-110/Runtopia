package com.ssafy.api.service;

import com.ssafy.common.exception.handler.UserNotFoundException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import com.ssafy.api.request.UserRegisterPostReq;
import com.ssafy.db.entity.User;
import com.ssafy.db.repository.UserRepository;
import org.springframework.transaction.annotation.Transactional;

import java.util.Optional;
//import com.ssafy.db.repository.UserRepositorySupport;

/**
 * 유저 관련 비즈니스 로직 처리를 위한 서비스 구현 정의.
 */
@Service("userService")
public class UserServiceImpl implements UserService {
    @Autowired
    UserRepository userRepository;

//	@Autowired
//	UserRepositorySupport userRepositorySupport;

    @Autowired
    PasswordEncoder passwordEncoder;

    @Override
    public User createUser(UserRegisterPostReq userRegisterInfo) {
        User user = new User();

        user.setUserId(userRegisterInfo.getUserId());
        // 보안을 위해서 유저 패스워드 암호화 하여 디비에 저장.
        user.setPassword(passwordEncoder.encode(userRegisterInfo.getPassword()));
        user.setNickname(userRegisterInfo.getNickname());
        user.setGender(userRegisterInfo.getGender());

        return userRepository.save(user);
    }


    @Override
    public User getUserById(String userId) {
        Optional<User> userOptional = userRepository.findByUserId(userId);
        //return userOptional.orElse(null);

        return userOptional
                .orElseThrow(() -> new UserNotFoundException("User not found with id: " + userId));
    }

    @Override
    public boolean authenticateUser(String userId, String password) {
        User user = getUserById(userId);
        if (user == null) {
            return false;
        }
        return user.getPassword().equals(password);
    }

    @Override
    public User updateUser(User user) {
        // Implement the logic for updating the user in the database
        return userRepository.save(user);
    }

    @Override
    @Transactional
    public void deleteUserById(String userId) {
        Optional<User> userOptional = userRepository.findByUserId(userId);

        if (!userOptional.isPresent()) {
            //throw new UserNotFoundException();
        }
        userRepository.deleteByUserId(userId);
    }

    @Override
    public boolean isNicknameAvailable(String nickname) {
        User user = userRepository.findByNickname(nickname);
        return user == null;
    }
}
