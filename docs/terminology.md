# Workshop Terminology

A plain-language glossary of terms used throughout this workshop's labs and presentations. It's written for non-technical attendees. If you're new to software development or Claude Code, start here whenever you hit an unfamiliar word.

Terms are grouped by category and listed alphabetically within each group.

## Claude Code & AI

- **Agent / Agentic** — An AI that doesn't just answer questions but takes multi-step actions toward a goal: reading files, running commands, editing code, and checking its own results. Claude Code is agentic, so you direct it with a task instead of accepting one suggestion at a time.
- **CLAUDE.md** — A plain text file of instructions and project knowledge (build commands, coding standards, things to avoid) that Claude Code reads at the start of every session. It's the project's "memory". Lab 2 covers it.
- **Claude Code** — Anthropic's command-line AI assistant for software development. It reads your codebase, proposes changes, and runs commands, with your approval.
- **Context Window** — The amount of text (instructions, code, conversation history) the AI model can "see" and consider at once. When it fills up, quality drops, so you manage it with commands like `/context` and `/compact`.
- **Checkpoint / Rewind** — Claude Code saves a restore point before it edits files, so you can undo its changes to files and the conversation without using Git (`/rewind`). Lab 6 covers it.
- **Hallucination** — When an AI confidently produces information or code that is incorrect or made up. It's why you review changes and run tests instead of trusting output blindly.
- **Harness** — The surrounding application that runs an AI model and lets it act: reading files, running commands, calling tools. Claude Code is a harness. The model only generates text; the harness decides which tools it may use and carries out the actions.
- **Headless Mode** — Running Claude Code non-interactively with `claude -p "your prompt"`, so scripts and automated pipelines can use it. Lab 13 covers it.
- **Hook** — A small script that Claude Code runs automatically at set moments (for example, before or after it edits a file), configured in `.claude/settings.json`. Lab 9 covers it.
- **MCP (Model Context Protocol)** — An open standard that lets Claude Code connect to external systems and data sources (such as GitHub) in a consistent way. Lab 10 covers it.
- **Model** — The underlying AI (for example, Claude Sonnet) that generates the responses. You can switch models with `/model`.
- **Permission Mode** — A setting that controls how much Claude Code may do without asking: Manual (ask for each action), Accept edits, Plan (read-only), and Auto. Shift+Tab cycles through them.
- **Plan Mode** — A permission mode where Claude Code can read and propose a plan but can't change any files, until you approve. Lab 4 covers it.
- **Plugin** — A shareable bundle of Claude Code customizations (skills, subagents, hooks, and MCP servers) that others can install. Lab 15 covers it.
- **Prompt** — The instructions or question you give to an AI assistant to get the response or action you want.
- **Rules (`.claude/rules/`)** — Instruction files that load only when Claude Code works on matching files, such as .NET files or Spring Boot files.
- **Skill** — A packaged set of instructions (a `SKILL.md` file) that Claude Code loads when a task needs it, or that you invoke with `/name`. Lab 11 covers it.
- **Slash Command** — A shortcut typed with a leading `/` (like `/help`, `/context`, or `/compact`) that tells Claude Code to perform a specific action.
- **Subagent** — A specialized helper that Claude Code can hand a task to, with its own instructions, its own tools, and its own separate context. Lab 12 covers it.
- **Token** — A small chunk of text (roughly a word fragment) that AI models read and write. Usage and limits are measured in tokens.

## Development Practices & Architecture

- **Aggregate** — A cluster of related data and rules that are treated as a single unit and always kept consistent together. In the TaskManager app, `Task` is the aggregate: changes go through it so the data can't end up in a contradictory state.
- **Bounded Context** — A clear boundary around a specific business area where terms and rules have one consistent meaning. "Customer" can mean different things to Sales and to Support, so each gets its own boundary.
- **CI/CD (Continuous Integration / Continuous Deployment)** — Automated processes that build, test, and release code changes frequently and reliably. Every change is compiled and tested as soon as it's pushed, so problems are caught in minutes.
- **Clean Architecture** — A way of organizing code in layers (Domain, Application, Infrastructure, API) so core business logic never depends on technical details like databases or web frameworks. Dependencies point inward only. The TaskManager app uses it in both stacks.
- **DDD (Domain-Driven Design)** — An approach to software that models the code around the real business concepts and language that experts in that field use. Entities, Value Objects, Aggregates, and Bounded Contexts are its building blocks.
- **Dependency Injection** — A technique where a piece of code is given the tools it needs from outside instead of creating them itself. It makes code easier to test, because a real database can be swapped for a fake one.
- **Entity** — A business object with its own identity that can change over time while still being "the same thing". A specific Task is an entity: its title or status can change, but it's the same task because of its ID.
- **Integration Test** — A test that checks whether multiple parts of a system work correctly together (for example, the API and a database). Slower than unit tests, so teams write fewer of them.
- **Red-Green-Refactor** — The cycle at the heart of TDD: write a small failing test (red), write just enough code to pass it (green), then clean up without changing behavior (refactor).
- **Refactoring** — Improving the internal structure of code without changing what it does from the outside. Tests confirm nothing broke. Lab 5 does this to the intentionally messy `LegacyTaskProcessor`.
- **TDD (Test-Driven Development)** — Writing a test for a small piece of behavior before the code that implements it. You get working code and a test suite as a byproduct. Lab 3 covers it.
- **Unit Test** — A small, fast, automated test of a single piece of logic in isolation, with no database or network. Healthy codebases have many.
- **User Story** — A short description of a feature from the user's point of view ("As a ... I want ... so that ..."), usually with acceptance criteria that say when it's done.
- **Value Object** — A piece of data defined entirely by its values, with no identity of its own, such as a due date or a priority. Two equal values are interchangeable.

## Tools & Environment

- **CLI (Command-Line Interface)** — A way of using software by typing text commands instead of clicking buttons. Claude Code is a CLI tool.
- **Command Palette** — VS Code's search box for every feature, opened with `Ctrl+Shift+P` (Windows/Linux) or `Cmd+Shift+P` (Mac).
- **Dev Container** — A pre-configured development environment (tools and dependencies included) that runs the same way for everyone, inside a container. The workshop repo ships three.
- **Extension** — An add-on that gives VS Code extra abilities, such as the Claude Code extension.
- **Git** — A version control system that tracks changes to files over time, letting many people collaborate on the same codebase safely.
- **GitHub** — A web platform for hosting Git repositories and collaborating on code: reviews, issues, and automation.
- **IDE (Integrated Development Environment)** — An application that combines a code editor, tools, and helpers in one place (for example, VS Code).
- **Integrated Terminal** — A terminal window inside VS Code, opened with Terminal → New Terminal.
- **jq** — A small command-line tool for reading and filtering JSON data. The Day 2 hook and automation labs use it.
- **Node.js / npx** — A JavaScript runtime. The workshop needs it only so `npx` can run an MCP server in Lab 10.
- **Terminal** — A text-based window used to type and run commands directly to the computer.
- **VS Code** — Visual Studio Code, the free code editor used throughout this workshop.

## Git & GitHub Concepts

- **Branch** — A separate, parallel line of work in a Git repository, used to make changes without affecting the main code until they're ready. You'll work on your own `workshop/your-name` branch.
- **Clone** — Making a local copy of a Git repository on your own computer.
- **Commit** — A saved snapshot of changes in a Git repository, with a message describing what changed.
- **Conventional Commits** — A convention for commit messages that starts with a type, such as `feat:`, `fix:`, `test:`, or `docs:`.
- **Fork** — Your own copy of someone else's GitHub repository, under your account, so you can make changes without affecting the original. The workshop requires one.
- **Issue** — A tracked task, bug, or question on GitHub.
- **Main** — The default branch of a repository, holding the shared, trusted version of the code.
- **Origin / Upstream** — Names for remote copies of a repository. `origin` is your fork; `upstream` is the original workshop repo you pull updates from.
- **Pull Request (PR)** — A request to merge one branch into another, typically reviewed by others before being accepted.
- **Push / Pull** — Upload your commits to GitHub (push) or download others' commits from it (pull).
- **Remote** — A copy of a repository hosted elsewhere, such as on GitHub.
- **Repository (Repo)** — A storage location for a project's code and its full history of changes.
- **Staging (`git add`)** — Choosing which changed files go into your next commit.

## Software & Web Concepts

- **API (Application Programming Interface)** — A defined way for different pieces of software to talk to each other, like a menu of requests one program can make to another.
- **Endpoint** — A specific address (URL) that an API exposes, which other software can call to request or send data, for example `POST /tasks`.
- **Framework vs. Library** — A library is reusable code you call when you need it; a framework provides the overall structure and calls your code for you.
- **JSON** — A text format for structured data, used by APIs and by many Claude Code settings files.
- **Maven / .NET SDK** — The build tools for the Spring Boot and .NET stacks. They compile the code and run the tests.
- **Minimal API** — A simplified style (used in .NET) for building APIs with less boilerplate code.
- **REST** — A common style for web APIs where URLs name things (like tasks) and standard methods (GET, POST, PUT, DELETE) act on them.
- **Spring Boot** — A popular Java framework for building web applications and APIs. It's one of the two workshop stacks, alongside .NET.
