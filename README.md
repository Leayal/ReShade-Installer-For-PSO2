# ReShade shader installer for PSO2 game
 Just as the title said, this is a robust installer made just for the game pso2. It aims to help Tweaker users to use ReShade with ease.
 
## Credits
- CeeJayDK for his awesome SweetFX shader effect files.
- ReShade team and all ReShade's contributors for the hooking and shader effect files.
 
## Gameguard problem???
 When I use ReShade2 and SweetFX's FXAA hook by placing the hooking file as `d3d9.dll` in `pso2_bin`, the PSO2's Gameguard just crash the game with error `NP1013`. But then ReShade3 (to be specific, it's ReShade 3.0.8) came out, I can use it by placing the file as `d3d9.dll` just fine without triggering the Gameguard, although some people still suffer that, I don't know why. This is the reason why `Wrapper installation` and `Safe installation` exist.

## Why ReShade3 can be used as if it's a pso2 plugins???
 I don't really know about this. But this is the guess back then when I mess up with my game:
 - pso2h.dll use `LoadLibrary` API to load the .dll files in `Plugins` folder.
 - ReShade3 has dll entry point (this is the reason why `LoadLibrary` worked) while also having all the proxy DirectX and OpenGL api (This is the reason why the gameguard doesn't crash anymore if you put the .dll as `d3d9.dll`)
 
 This is just my vague guess. I don't confirm anything.
 
## Side notes:
If you can use `wrapper Installation` just fine, then just use it, ReShade3 is designed to be working in that way.

But if Gameguard complains with error `NP1013`, delete the old installation by remove/rename/move the `d3d9.dll` file away from pso2_bin, then use `Safe Installation` instead.


# Happy graphics ~~messing~~ modding.
