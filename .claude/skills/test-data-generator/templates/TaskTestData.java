// Template for the test-data-generator skill.
// Matches the starting domain model in src-springboot/taskmanager-domain.
// Adapt it when the model gains new members (priority, due date, assignee, ...).
package com.example.taskmanager.domain.tasks;

import java.util.List;
import java.util.stream.Stream;

import org.junit.jupiter.params.provider.Arguments;

public final class TaskTestData {

    private TaskTestData() {
    }

    public static Task validTask() {
        return Task.create(
            "Fix login timeout on mobile",
            "Users are logged out after 30 seconds on iOS Safari.");
    }

    public static Task inProgressTask() {
        Task task = validTask();
        task.start();
        return task;
    }

    public static Task completedTask() {
        Task task = validTask();
        task.complete();
        return task;
    }

    public static Task cancelledTask() {
        Task task = validTask();
        task.cancel();
        return task;
    }

    public static List<Task> sampleTasks() {
        return List.of(
            Task.create("Write ADR for caching strategy", "Compare in-memory and distributed cache options."),
            Task.create("Add pagination to task list endpoint"),
            inProgressTask(),
            completedTask(),
            cancelledTask());
    }

    // Edge cases: use with @ParameterizedTest + @MethodSource.
    public static Stream<String> invalidTitles() {
        return Stream.of("", "   ", "\t\n", "x".repeat(201));
    }

    public static Stream<String> unusualButValidTitles() {
        return Stream.of("A", "x".repeat(200), "Mise à jour du système 🚀", "タスクを完了する");
    }

    public static Stream<Arguments> invalidTransitions() {
        return Stream.of(
            Arguments.of(completedTask(), "complete"),
            Arguments.of(cancelledTask(), "complete"),
            Arguments.of(completedTask(), "cancel"),
            Arguments.of(inProgressTask(), "start"));
    }
}
