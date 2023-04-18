package com.ssafy.db.entity;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;
import java.time.LocalDateTime;
import java.time.LocalTime;

@Entity
@Getter
@Setter
public class Achieve {

    @Id
    @Column
    @GeneratedValue(strategy = GenerationType.IDENTITY) // auto_increment
    Long id;

    @Column
    String userId;

    @Column
    Long winCount;

    @Column
    Long CoinCount;

    @Column
    LocalTime BestTime;

    @Column
    LocalTime playTime;

    @Column
    String tier;

    @Column
    Long mmr;

    public void setAchieve(Achieve achieve) {
        Achieve achieve1 = new Achieve();

        achieve1.setUserId(achieve.userId);
        achieve1.setBestTime(achieve.BestTime);
        achieve1.setCoinCount(achieve.getCoinCount());
    }
}
