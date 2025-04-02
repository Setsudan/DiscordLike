package net.ethlny.discordhetic.discord_backend_hetic.services.avatar;

import com.amazonaws.services.s3.AmazonS3;
import com.amazonaws.services.s3.model.PutObjectRequest;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.UUID;

@Service
public class AvatarService {

    private final AmazonS3 amazonS3;

    @Value("${s3.bucket}")
    private String bucketName;

    @Value("${s3.urlPrefix}")
    private String s3UrlPrefix;

    public AvatarService(AmazonS3 amazonS3) {
        this.amazonS3 = amazonS3;
    }

    public String uploadAvatar(MultipartFile file) throws IOException {
        // Generate a unique filename
        String fileName = UUID.randomUUID().toString() + "_" + file.getOriginalFilename();

        // Convert MultipartFile to File
        File convFile = convertMultiPartToFile(file);

        // Upload file to S3
        amazonS3.putObject(new PutObjectRequest(bucketName, fileName, convFile));

        // Delete temporary file
        convFile.delete();

        // Construct and return the public URL
        return s3UrlPrefix + "/" + bucketName + "/" + fileName;
    }

    private File convertMultiPartToFile(MultipartFile file) throws IOException {
        File convFile = new File(System.getProperty("java.io.tmpdir") + "/" + file.getOriginalFilename());
        try (FileOutputStream fos = new FileOutputStream(convFile)) {
            fos.write(file.getBytes());
        }
        return convFile;
    }
}
