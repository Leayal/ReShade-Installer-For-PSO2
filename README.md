# ====Discontinued====
## As you already know, ReShade works just fine with the game now. You can simply use ReShade's installer to install ([Get it from ReShade's homepage](https://reshade.me/#download))

# ReShade shader installer for PSO2 game
 Just as the title said, this is a robust installer made just for the game pso2. It aims to help Tweaker users to use ReShade with ease.
 
## Credits
- CeeJayDK for his awesome SweetFX shader effect files.
- ReShade team and all ReShade's contributors for the hooking and shader effect files.

## Uninstallation
Sadly, I'll admit that I was so lazy to make an uninstaller. But here's the guide on the Wiki page: [Uninstallation Guide](https://github.com/Leayal/ReShade-Installer-For-PSO2/wiki#uninstallation)
 
## Gameguard problem???
 When I use ReShade2 and SweetFX's FXAA hook by placing the hooking file as `d3d9.dll` in `pso2_bin`, the PSO2's Gameguard just crash the game with error `NP1013`. But then ReShade3 (to be specific, it's ReShade 3.1.1) came out, I can use it by placing the file as `d3d9.dll` just fine without triggering the Gameguard, although some people still suffer that, I don't know why. This is the reason why `Wrapper installation` and `Safe installation` exist.
 
## Side notes:
* If you can use `wrapper Installation` just fine, then just use it, ReShade3 is designed to be working in that way. But if Gameguard complains with error `NP1013`, delete the old installation by remove/rename/move the `d3d9.dll` file away from pso2_bin, then use `Safe Installation` instead.
* The installer have packages inside the compiled .exe file, which mean the shader effect files may be out-dated compare to [ReShade's shader sources](https://github.com/crosire/reshade-shaders). You can download the files here to update your existing files if you feel like they're out-dated.
* ReShade menu or ReShade's shortcut key not working???? --->[Take a look at this Wiki](https://github.com/Leayal/ReShade-Installer-For-PSO2/wiki#why-reshade-doesnt-listen-my-keyboard-cant-open-reshade-menu-or-take-screenshot-with-reshade-or-toggle-onoff-an-effect)<---

# Happy graphics ~~messing~~ modding.
