package com.ssafy.api.controller;

import com.ssafy.api.service.FileServiceImpl;
import com.ssafy.common.error.ErrorCode;
import com.ssafy.common.exception.CustomException;
import io.swagger.annotations.Api;
import lombok.RequiredArgsConstructor;
import org.springframework.core.io.Resource;
import org.springframework.core.io.UrlResource;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.net.MalformedURLException;
import org.springframework.http.HttpHeaders;

/**
 * 유저 관련 API 요청 처리를 위한 컨트롤러 정의.
 */
@Api(value = "파일 API", tags = {"File"})
@RestController
@RequiredArgsConstructor
@RequestMapping("/api/v1/file")
public class FileController {
    private final FileServiceImpl fileServiceImpl;
    @GetMapping("/download/{filename}")
    public ResponseEntity<Resource> downloadFile(@PathVariable String filename) throws MalformedURLException {
        //file:/Users/.../uuid 로 작성된 파일명
        //UrlResource 가 실제 서버의 파일 경로에 있는 파일을 찾아온다.
        Resource resource = new UrlResource("file:" + fileServiceImpl.fileDownload(filename));
        //파일이 존재하지 않으면 예외처리
        if(!resource.isReadable())
            throw new CustomException(ErrorCode.FILE_NOT_FOUND);

        // Set the Content-Disposition header for downloading the file
        HttpHeaders headers = new HttpHeaders();
        headers.add(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=" + resource.getFilename());

        return ResponseEntity.ok()
                .headers(headers)
                .body(resource);
    }

}
