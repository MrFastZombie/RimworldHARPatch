# Rimworld HAR CheckMaskShader Patch

This is an experimental patch for [Humanoid Alien Races](https://github.com/erdelf/AlienRaces) for Rimworld 1.6 that caches the results of CheckMaskShader() to speed up its calls.

## Why?

In my modlist (now 1000 mods with this one), there was an issue of HAR calling HeadGraphicForPrefix() many times per second, and each call goes through all of my 58,000 texture files as far as I can tell. Most of the time was spent on the CheckMaskShader() method within it, and caching the results for that method was simple.

## Should I use this?

To be completely honest, I don't understand the consequences of this patch 100%. I have tested it by running a dev quick test world with this patch enabled and found no obvious issues, but I don't know what issues might arise later.

As it stands currently, shaders are cached once and never again until the game is reloaded.

I can't recommend relying on this unless you understand what it is doing and what problem it solves.

Also, if you have a light modlist then this patch will probably provide very little improvement.

## Does it work?

### Anecdotal remarks

Barring any issues discussed above, I did find that this helps improve performance for me personally.

Prior to running this patch, my game would drop tons of frames when zoomed out due to HeadGraphicForPrefix() being called more, and it seemed to be worse if HAR pawns were present though not exclusive to HAR pawns.

With the patch, zooming out can still reduce the frame rate but to a lesser degree.

### Test 1: No patch, 3 baseliners and 5 HAR pawns, Whole map in view. 1 minute of data collection

TPS was around 40, with many fluctuations.

FPS was 20-35.

![Test results showing 830us per call avg](./test1.png)

### Test 2: Patch without tick based renewal, 3 baseliners and 5 HAR pawns, Whole map in view. 1 minute of data collection

TPS stayed around 60.

FPS averaged 40 while zoomed out.

![Test results showing 40us per call avg](./test2.png)

### Conclusion

The patch appears to improve performance with a huge modlist such as mine. The call time improved by 20x.  Do note that actual gameplay FPS may be higher, as I've noted running the profiler may reduce framerate as well. After running with the patch, I've also noticed less stuttering. I'd say that this patch has been successful in fixing this performance concern with my modlist.
