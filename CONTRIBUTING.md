# Contributing

Thanks for helping improve the Claude Code workshop. This guide covers feedback, issues, and changes to labs, slides, and code.

By participating you agree to follow the [Code of Conduct](CODE_OF_CONDUCT.md).

## Give feedback or report a problem

Open a [GitHub issue](../../issues). Useful things to include:

- The lab number and step (for example, "Lab 5, step 3") or the file path
- Your stack: .NET or Spring Boot
- What you expected, what happened, and any error text
- Your Claude Code version (`claude --version`)

## Propose a change

1. Open an issue first for anything beyond a typo, so we can agree on scope.
2. Create a branch from `main`. Never commit to `main` directly. Workshop participants use `workshop/your-name`; maintainers can use a descriptive branch such as `docs/fix-lab-3-typo`.
3. Make the change following the conventions below.
4. Open a pull request using the matching template (see below).

### Conventions

- **Conventional Commits:** `feat:`, `fix:`, `test:`, `refactor:`, `docs:`, `chore:`.
- **TDD first** for code changes; write tests before implementation.
- **Clean Architecture:** dependencies point inward (API → Application → Domain).
- **No dead code:** delete unused code instead of commenting it out.
- **Keep both stacks in step.** If a lab changes for .NET, check whether the Spring Boot version needs the same change.
- **Don't add NuGet packages or Maven dependencies** without discussing it in the issue first.
- **Labs:** follow the structure of the existing guides in `docs/labs/`. Slides in `docs/presentations/` are Marp decks; re-render to check for overflow after editing.

### Build and test

```bash
dotnet build src-dotnet && dotnet test src-dotnet

cd src-springboot && mvn clean test
```

## Pull request templates

GitHub does not pick a template from a folder automatically. Add the template name to the URL when opening the PR:

- New lab or content: `…/compare/main...your-branch?expand=1&template=feature.md`
- Correction or fix: `…/compare/main...your-branch?expand=1&template=fix.md`

Or copy the text from `.github/PULL_REQUEST_TEMPLATE/` into the PR description.

## Merging

Maintainers merge with merge commits (no squash). Pull requests need a human review and are not auto-approved.
