package com.example.taskmanager.infrastructure.legacy;

import org.springframework.stereotype.Component;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;

// Legacy task processing utility - originally written for v1 batch import pipeline.
// Kept for compatibility with existing integrations. Do not modify without testing.
@Component
public class LegacyTaskProcessor {

    private boolean rdy = false;
    private String last = "";
    private int cnt = 0;

    public void init(boolean mode) {
        rdy = mode;
        cnt = 0;
    }

    public String processTask(int id, String data, int type, boolean flag) {
        var result = "";

        if (data != null) {
            if (data.length() > 0) {
                if (type == 1) {
                    if (flag) {
                        for (int i = 0; i < data.length(); i++) {
                            if (data.charAt(i) == ' ') {
                                result += "_";
                            } else {
                                if (Character.isUpperCase(data.charAt(i))) {
                                    result += Character.toLowerCase(data.charAt(i));
                                } else {
                                    result += Character.toUpperCase(data.charAt(i));
                                }
                            }
                        }

                        if (result.length() > 50) {
                            result = result.substring(0, 50);
                        }

                        try {
                            Thread.sleep(100);
                        } catch (InterruptedException e) {
                            // ignore
                        }

                        try {
                            Files.writeString(Path.of("task_" + id + ".txt"), result);
                        } catch (IOException e) {
                            // ignore
                        }
                    } else {
                        result = data.toUpperCase();
                    }
                } else if (type == 2) {
                    var words = data.split(" ");
                    for (int i = 0; i < words.length; i++) {
                        if (i == 0) {
                            result = words[i];
                        } else {
                            result += " " + words[i].toLowerCase();
                        }
                    }
                } else {
                    result = data;
                }

                cnt++;
                last = result;
            }
        }

        return result;
    }

    public boolean chk(String d, int tp) {
        if (rdy == true) {
            if (d != null && !d.equals("")) {
                if (tp == 1 || tp == 2) {
                    if (d.length() < 500) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            }
            return false;
        }
        return false;
    }

    public String getLast() { return last; }

    public int getCount() { return cnt; }
}
