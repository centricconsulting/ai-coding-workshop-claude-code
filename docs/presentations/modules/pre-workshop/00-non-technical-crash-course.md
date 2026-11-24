---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Non-Technical Crash Course

## Terminal, VS Code, Git & GitHub in 1 Hour

**Duration:** 60 Minutes
**Format:** Instructor-led, hands-on (pre-workshop session 1)
**Audience:** BAs, PMs, and other non-engineering participants

---

# Why This Session Exists

The workshop assumes you can already:

- Open a terminal (Claude Code runs there)
- Navigate VS Code
- Clone a repo, commit, push, pull
- Understand branches, pull requests, and issues

**Goal today:** working familiarity, not mastery, so you're ready for session 2 and Lab 1.

---

# Today's Journey

```
1. Welcome & why this matters      (3 min)
2. Terminal basics                 (10 min)
3. VS Code basics                  (12 min)
4. Git basics                      (15 min)
5. GitHub + fork, clone, branch    (15 min)
6. Wrap-up & readiness check       (5 min)
```

**Next:** session 2 (everyone) installs Claude Code and verifies the build.

---

<!-- _class: lead -->

# Module 1: Why This Matters

**Duration:** 3 minutes

---

# The Four Tools, in Plain English

- **Terminal**: how you type instructions instead of clicking. Claude Code runs here.
- **VS Code**: the editor where you read and review the code
- **Git**: "track changes" for an entire folder of files, with history
- **GitHub**: the website that hosts the shared code and enables collaboration

You don't need to become an engineer today.

Keep the [terminology glossary](../../../terminology.md) open as a reference.

---

<!-- _class: lead -->

# Module 2: Terminal Basics

**Duration:** 10 minutes

---

# Open a Terminal

- **Windows:** Search "PowerShell" or "Terminal" in the Start menu
- **Mac:** `Cmd+Space`, type "Terminal"

```bash
pwd          # Print Working Directory: "where am I?"
ls           # List files
cd Desktop   # Change Directory: move into a folder
cd ..        # Move up one level
```

---

# Terminal Key Concepts

- There's always a **current location**, like Explorer/Finder but text-based
- Type a command, press `Enter` to run it
- Spelling matters
- `↑` repeats your last command; `Tab` completes names

**Try it:** Move into a folder, then back out, using `cd`.

---

<!-- _class: lead -->

# Module 3: VS Code Basics

**Duration:** 12 minutes

---

# Open a Folder

- Launch VS Code
- **File → Open Folder...**
- VS Code always works on a **folder** ("workspace"), not a single file

---

# Tour the Sidebar

| Icon | Panel | Purpose |
|---|---|---|
| 📄 | Explorer | Browse files and folders |
| 🔍 | Search | Find text across all files |
| 🔀 | Source Control | See Git changes |
| 🧩 | Extensions | Install add-ons (Claude Code extension in session 2) |

---

# Integrated Terminal & Command Palette

- **Terminal → New Terminal** (or `` Ctrl+` ``): the same terminal, inside VS Code
- `Ctrl+Shift+P` / `Cmd+Shift+P` opens the **Command Palette**: find any feature by name

**Checkpoint:** Everyone has a terminal open inside VS Code.

---

<!-- _class: lead -->

# Module 4: Git Basics

**Duration:** 15 minutes

---

# Git Vocabulary

| Term | Plain-English Meaning |
|---|---|
| Repository | A folder that Git is tracking |
| Commit | A saved snapshot, with a message |
| Branch | An independent, safe line of work |
| Clone | Download a copy of a repo |
| Push / Pull | Upload / download commits |

---

# The Core Loop

```bash
git status               # What has changed?
git add <file>           # Stage a file for the next commit
git commit -m "message"  # Save a snapshot
git push                 # Upload commits to GitHub
git pull                 # Download latest commits
```

**Practice first** in a throwaway folder: `git init`, create a file, `status`, `add`, `commit`, `log`.

---

# Branches in 60 Seconds

```bash
git checkout -b my-experiment   # Create and switch to a new branch
git branch                      # Which branch am I on? (* marks it)
git checkout main               # Switch back
```

Working on your own branch means you can't break anyone else's work.

---

<!-- _class: lead -->

# Module 5: GitHub Basics + Fork, Clone, Branch

**Duration:** 15 minutes

---

# GitHub.com in 4 Concepts

- **Repository page**: the web view of a repo (code, files, history)
- **Fork**: your own copy of someone else's repo
- **Pull Request (PR)**: a request to merge your branch, reviewed by teammates
- **Issue**: a tracked task, bug, or question

---

# Why a Fork?

Labs 7, 8, 10, 13, and 15 push to, and query, a GitHub repo **you control**.

Your fork is that repo.

1. Open [github.com/centricconsulting/ai-coding-workshop-claude-code](https://github.com/centricconsulting/ai-coding-workshop-claude-code)
2. Click **Fork**, and create it under your account

---

# Clone Your Fork & Branch

```bash
git clone https://github.com/YOUR-GITHUB-USERNAME/ai-coding-workshop-claude-code.git
cd ai-coding-workshop-claude-code

git remote add upstream https://github.com/centricconsulting/ai-coding-workshop-claude-code.git
git remote -v

git checkout -b workshop/your-name
code .
```

**Never commit directly to `main`.**

---

# Practice the Full Loop

```bash
echo "Joined the workshop" > my-notes.txt
git add my-notes.txt
git commit -m "docs: add my notes file"
git push -u origin workshop/your-name
```

Then find `my-notes.txt` on your branch on github.com.

---

<!-- _class: lead -->

# Module 6: Wrap-Up & Readiness Check

**Duration:** 5 minutes

---

# Readiness Checklist

- [ ] I can open a terminal and use `cd`
- [ ] I can open a folder in VS Code and find the integrated terminal
- [ ] I know what commit, push, pull, and branch mean
- [ ] I've forked the repo and cloned my fork
- [ ] `git remote -v` shows `origin` (my fork) and `upstream`
- [ ] I'm on `workshop/your-name` and my push appeared on GitHub

---

# What's Next

**Session 2 (everyone):** install Claude Code, sign in, and verify your stack builds. We walk through the [Local Setup Guide](../../../LOCAL_SETUP.md), or the [devcontainer guide](../../../../.devcontainer/README.md) if you can use devcontainers.

The fork, clone, and branch step is already done.

**Then:** [Lab 1: First Contact](../../../labs/lab-01-first-contact.md)
