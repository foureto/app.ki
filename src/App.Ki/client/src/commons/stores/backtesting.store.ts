import { createEffect, createEvent } from "effector";


const backtestsRequested = createEvent();

const getBacktestsFx = createEffect(() => BacktestingService.get())