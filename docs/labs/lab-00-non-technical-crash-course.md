# Lab 0: Non-Technical Crash Course (Terminal, VS Code, Git, GitHub)

**Duration**: 60 minutes
**When**: Pre-workshop session 1 (live, the week before the workshop)
**Audience**: Business analysts, product managers, and other non-engineering participants who need just enough tooling literacy to take the main workshop
**Learning Objectives**:

- Run basic commands in a terminal with confidence
- Navigate VS Code: open a folder, use the sidebar, use the integrated terminal
- Understand and perform the core Git workflow: clone, status, add, commit, push, pull, branch
- Understand GitHub basics: repositories, forks, branches, pull requests, issues
- Fork this workshop repository, clone your fork, and create your personal branch

> **Facilitator note**: This lab assumes zero prior command-line or Git experience. Keep the pace brisk — the goal is working familiarity, not mastery. Participants should leave with their fork cloned and their own branch created. Installing Claude Code, authenticating, and building the code are **not** part of this session: everyone does those in pre-workshop session 2, a walkthrough of the [Local Setup Guide](../LOCAL_SETUP.md) (or the [devcontainer guide](../../.devcontainer/README.md) for people who can use devcontainers).

> 📖 **Tip**: Keep the [Workshop Terminology glossary](../terminology.md) open as a reference. It defines every tooling and development term used in this lab (and beyond) in plain language.

---

## Overview

The main workshop assumes participants can open a terminal, navigate VS Code, and perform basic Git and GitHub operations. Claude Code itself runs in a terminal, and every lab ends with Git. This lab closes that gap in one hour so non-engineering participants arrive at session 2 and Lab 1 ready to focus on Claude Code, not on tooling.

**Time Budget**:

| Segment | Duration |
|---|---|
| 1. Welcome & why this matters | 3 min |
| 2. Terminal basics | 10 min |
| 3. VS Code basics | 12 min |
| 4. Git basics | 15 min |
| 5. GitHub basics + fork, clone, and branch | 15 min |
| 6. Wrap-up & readiness check | 5 min |

---

## Prerequisites

- ✅ Laptop where you can install software
- ✅ [VS Code](https://code.visualstudio.com/) installed
- ✅ [Git](https://git-scm.com/downloads) installed (Windows: choose the default options in the installer)
- ✅ A [GitHub.com](https://github.com/) account, signed in

You do **not** need a Claude account, Claude Code, .NET, or Java for this session. If anyone is missing the four items above, install them now — the facilitator should pause here until everyone is ready.

---

## Step 1: Why This Matters (3 min)

Before typing anything, set expectations:

- **Terminal** = how you type instructions to your computer instead of clicking. Claude Code runs here.
- **VS Code** = the editor where you'll read the code Claude Code writes and review its changes
- **Git** = the tool that tracks changes to files over time (like "track changes" for a whole folder of files)
- **GitHub** = the website that hosts the shared copy of the code and enables collaboration (pull requests, reviews)

You don't need to become an engineer today — just comfortable enough to follow along in the workshop.

---

## Step 2: Terminal Basics (10 min)

### 2.1 Open a Terminal

- **Windows**: Search for "PowerShell" or "Terminal" in the Start menu (if you installed Git for Windows, "Git Bash" also works)
- **Mac**: Open "Terminal" from Spotlight (`Cmd+Space`, type "Terminal")

### 2.2 Try These Commands

```bash
pwd          # Print Working Directory: "where am I?"
ls           # List files in the current folder
cd Desktop   # Change Directory: move into a folder
cd ..        # Move up one folder level
```

> `ls` works on Mac, in Git Bash, and in PowerShell. On older Windows Command Prompt, use `dir` instead.

### 2.3 Key Concepts

- The terminal always has a **current location** (like File Explorer or Finder, but text-based)
- Commands are typed, then run with `Enter`
- Spelling matters (and on Mac, so does capitalization). Typos are the #1 source of "it doesn't work"
- `↑` (up arrow) repeats your last command, and `Tab` completes file and folder names

**Expected outcome**: Everyone can open a terminal and move into a folder and back out using `cd`.

---

## Step 3: VS Code Basics (12 min)

### 3.1 Open a Folder

- Launch VS Code
- **File → Open Folder...** and pick your Desktop (or any folder)
- Point out: VS Code always works on a **folder** (a "workspace"), not a single file

### 3.2 Tour the Sidebar

| Icon | Panel | Purpose |
|---|---|---|
| 📄 | Explorer | Browse files and folders |
| 🔍 | Search | Find text across all files |
| 🔀 | Source Control | See Git changes (you'll use this in Step 4) |
| 🧩 | Extensions | Install add-ons (you'll add the Claude Code extension in session 2) |

### 3.3 Open the Integrated Terminal

- Menu: **Terminal → New Terminal** (or `` Ctrl+` ``)
- This is the **same terminal** from Step 2, just inside VS Code — no need to switch windows. In the workshop you'll start Claude Code from here.

### 3.4 Use the Command Palette

- Press `Ctrl+Shift+P` (Windows/Linux) or `Cmd+Shift+P` (Mac)
- Type "new terminal" and pick **Terminal: Create New Terminal**
- The Command Palette is how you find any VS Code feature by name

**Expected outcome**: Everyone has opened a folder, found the integrated terminal, and opened the Command Palette.

---

## Step 4: Git Basics (15 min)

> **Analogy**: Git is "track changes" for an entire folder of files, with a permanent history you can rewind.

### 4.1 Core Vocabulary

| Term | Plain-English Meaning |
|---|---|
| Repository ("repo") | A folder that Git is tracking |
| Commit | A saved snapshot of changes, with a message describing why |
| Branch | An independent line of work, like a copy you can experiment on safely |
| Clone | Download a copy of a repo from GitHub to your computer |
| Push / Pull | Upload / download commits between your computer and GitHub |

### 4.2 Hands-On: A Practice Repo

First, tell Git who you are. Do this once per computer. Use the email address on your GitHub account:

```bash
git config --global user.name "Your Name"
git config --global user.email "you@example.com"
```

> If you skip this, Git may guess a name and email from your computer (on a Mac, something like `you@MacBook.local`), or refuse to commit with "Please tell me who you are". Either way, set it now.

Then practice on a throwaway folder so nothing you do affects the workshop repo:

```bash
cd ~/Desktop
mkdir git-practice
cd git-practice
git init -b main                  # Turn this folder into a repo, with a branch called main
echo "hello" > notes.txt          # Create a file
git status                        # "What has changed since my last save?"
git add notes.txt                 # Stage the file for the next commit
git commit -m "Add notes file"    # Save a snapshot with a description
git log --oneline                 # See the history
```

> If `git init -b main` reports an error about `-b`, your Git is older than 2.28. Run `git init` and then `git branch -m main` instead.

Run each command one at a time and discuss what happened. `push` and `pull` need a GitHub repo, so you'll try them in Step 5.

### 4.3 Branches in 60 Seconds

```bash
git checkout -b my-experiment     # Create a new branch and switch to it
git branch                        # List branches (* marks the current one)
git checkout main                 # Switch back to the main line of work
```

- Working on your own branch means you can't break anyone else's work
- This is exactly what you'll do in Step 5 with the real workshop repo

**Expected outcome**: Everyone can explain `status`, `add`, `commit`, `push`, `pull`, and `branch` in plain language.

---

## Step 5: GitHub Basics + Fork, Clone, and Branch (15 min)

### 5.1 GitHub.com in 4 Concepts

- **Repository page**: the web view of a repo — code, files, history
- **Fork**: your own copy of someone else's repo, under your GitHub account
- **Pull Request (PR)**: a request to merge your branch into `main`, where teammates review your changes
- **Issue**: a tracked task, bug, or question — how teams plan and discuss work

Briefly show a real PR and Issue in a browser if possible.

### 5.2 Fork the Workshop Repository

> **Forking is required.** Several labs (7, 8, 10, 13, and 15) push to and query a GitHub repo that you control. Your fork is that repo.

1. Go to [github.com/centricconsulting/ai-coding-workshop-claude-code](https://github.com/centricconsulting/ai-coding-workshop-claude-code)
2. Click **Fork** in the top-right corner and create the fork under your account

### 5.3 Clone Your Fork

Clone **your fork** (the URL has your username in it, not `centricconsulting`):

```bash
cd ~/Documents
git clone https://github.com/YOUR-GITHUB-USERNAME/ai-coding-workshop-claude-code.git
cd ai-coding-workshop-claude-code
```

If you're asked to sign in, follow the browser prompt. If sign-in fails, see Troubleshooting below.

### 5.4 Add the Upstream Remote

This lets you pull workshop updates later:

```bash
git remote add upstream https://github.com/centricconsulting/ai-coding-workshop-claude-code.git
git remote -v        # Shows origin (your fork) and upstream (the original)
```

### 5.5 Create Your Personal Branch

```bash
git checkout -b workshop/your-name
git branch           # * should be next to workshop/your-name
```

_Replace `your-name` with your first name or GitHub username._ **Never commit directly to `main`.**

### 5.6 Open It in VS Code and Try a Commit

```bash
code .
```

In the VS Code terminal, make a tiny change you can safely keep, then practice the full loop:

```bash
echo "Joined the workshop" > my-notes.txt
git status
git add my-notes.txt
git commit -m "docs: add my notes file"
git push -u origin workshop/your-name
```

Then open your fork on github.com, switch to your branch from the branch menu, and confirm `my-notes.txt` is there.

**Expected outcome**: Fork cloned, `origin` and `upstream` configured, personal branch created and pushed.

---

## Step 6: Wrap-Up & Readiness Check (5 min)

### Readiness Checklist

- [ ] I can open a terminal and move between folders with `cd`
- [ ] I can open a folder in VS Code and find the integrated terminal
- [ ] I know what `commit`, `push`, `pull`, and `branch` mean
- [ ] I have forked the workshop repo and cloned my fork
- [ ] `git remote -v` shows both `origin` (my fork) and `upstream`
- [ ] I'm on my own `workshop/your-name` branch and my push appeared on GitHub

### What's Next

Join **pre-workshop session 2**, where everyone installs and verifies their setup by walking through the [Local Setup Guide](../LOCAL_SETUP.md) (or the [devcontainer guide](../../.devcontainer/README.md)). You've already done its fork, clone, and branch step (Step 7), so skip it. After that, the first lab is **[Lab 1: First Contact](lab-01-first-contact.md)**.

---

## Troubleshooting

### "git: command not found"

Git isn't installed or isn't on your PATH. Install from [git-scm.com](https://git-scm.com/downloads) and open a **new** terminal.

### Asked for a password on `git push` or `git clone`

GitHub no longer accepts account passwords on the command line. Use the HTTPS URL and sign in through the browser window that opens (Git Credential Manager does this on Windows and in recent Git for Mac). If no browser window appears, ask the facilitator about a [personal access token](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens) or the GitHub CLI (`gh auth login`).

### "Repository not found" when cloning

Check the URL has **your** GitHub username and that the fork finished creating on github.com. Copy the URL from the green **Code** button on your fork.

### VS Code doesn't recognize the `code` command

Open VS Code, press `Cmd+Shift+P` / `Ctrl+Shift+P`, run **Shell Command: Install 'code' command in PATH** (Mac), then open a new terminal. On Windows, reinstall VS Code and keep "Add to PATH" checked.

### "Please tell me who you are" on commit

Run the two `git config --global` commands from Step 4.2.

### `cd ~/Desktop` says the path doesn't exist

Some Windows setups move the Desktop folder into OneDrive. Run `cd ~` instead and create `git-practice` there. Any folder works.

### Typed a command wrong or lost in the terminal

Press `Ctrl+C` to cancel, run `pwd` to see where you are, and `ls` to see what's there. Nothing you do in a practice folder can hurt the workshop repo.

---

## Facilitator Tips

- Walk the room during Steps 2–5; non-technical learners get stuck on typos and window focus, not concepts
- Pair up confident and less-confident participants
- It's fine to skip deep explanations of staging (`git add`) vs. committing — the goal is muscle memory, not theory
- Expect the sign-in prompt on the first clone or push to be the most common blocker; have the personal access token fallback ready
- If you're running short, skip Step 4.3 and teach branches once in Step 5.5
