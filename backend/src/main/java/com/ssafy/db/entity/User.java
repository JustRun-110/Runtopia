package com.ssafy.db.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.ssafy.api.request.UserLoginPostReq;
import com.ssafy.api.request.UserRankingReq;
import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

/**
 * 유저 모델 정의.
 */
@Entity
@Getter
@Setter
public class User extends BaseEntity {
    @Id
    @Column
    @GeneratedValue(strategy = GenerationType.IDENTITY) // auto_increment
    Long id;
    @Column
    String userId;
    @Column
    String nickname;
    @Column
    Boolean gender;
    @JsonIgnore
    @JsonProperty(access = JsonProperty.Access.WRITE_ONLY)
    @Column
    String password;
    @Column
    Long mmr;


    public static User toUserEntity(UserLoginPostReq userLoginPostReq) {
        User user = new User();
        user.setUserId(userLoginPostReq.getUserId());
        //user.setPassword(userLoginPostReq.getPassword());
        user.setGender(userLoginPostReq.getGender());
        user.setNickname(userLoginPostReq.getNickname());
        return user;
    }
}
