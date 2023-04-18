package com.ssafy.db.repository;

import com.ssafy.db.entity.Achieve;
import com.ssafy.db.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;


/**
 * 랭킹 관련 디비 쿼리 생성을 위한 JPA Query Method 인터페이스 정의.
 */
@Repository
public interface RankingRepository extends JpaRepository<Achieve, Long> {
    Achieve findByUserId(String userId);

    //void save(Achieve achieve);
}
