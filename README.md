# Armada
This repository shows how I would code in production. These are some parts of the projects I have been working on as a freelance.
I am fully authorized to upload the client code to GitHub publicly with the limitation that I have to remove assets altogether along with server code.

## Details
- The project follows most of [C# Google Style Guide](https://google.github.io/styleguide/csharp-style.html)
- The project uses async/await with [UniTask](https://github.com/Cysharp/UniTask) instead of a traditional co-routine.
- The project doesn't use Singleton as you would expect. Instead, it uses the Service Locator pattern a lot.
