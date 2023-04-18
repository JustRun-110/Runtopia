package com.ssafy.common.exception.handler;

import com.ssafy.common.exception.CustomException;
import com.ssafy.common.model.response.BaseResponseBody;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

@RestControllerAdvice
@RequiredArgsConstructor
public class GlobalExceptionHandler {
    @ExceptionHandler(value = {CustomException.class})
    protected ResponseEntity<? extends BaseResponseBody> handleCustomException(CustomException ex) {
        return ResponseEntity.status(ex.getErrorCode().getStatus()).body(BaseResponseBody.of(ex.getErrorCode().getStatus(), ex.getErrorCode().getMessage()));
    }
}
