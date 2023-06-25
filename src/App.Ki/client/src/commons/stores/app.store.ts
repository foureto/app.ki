import { createEffect, createEvent, createStore, sample } from "effector";
import { IdentityService, User } from "../../services/IdentityService";

const userRequested = createEvent();
const setUserRequested = createEvent<User>();
const getUserFx = createEffect(() => IdentityService.me());

const $userStore = createStore<User | null>(null)
  .on(getUserFx.doneData, (_, p) => p?.data ?? null)
  .on(setUserRequested, (_, p) => p)
  .reset(userRequested);

sample({ clock: userRequested, target: getUserFx });

export { $userStore, userRequested, setUserRequested };
