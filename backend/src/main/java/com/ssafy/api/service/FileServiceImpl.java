package com.ssafy.api.service;

import com.ssafy.common.error.ErrorCode;
import com.ssafy.common.exception.CustomException;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

@Service
public class FileServiceImpl implements FileService {
    //application.properties에 app.upload.dir을 정의하고, 없는 경우에는 default값으로 user.home
    @Value("${app.upload.dir:${user.home}}")
    private String DOMAIN;
    private String uploadPath;

    public String fileDownload(String fileName) {
        uploadPath = DOMAIN + File.separator + "files" + "/" + fileName;
        return uploadPath;

    }

}
