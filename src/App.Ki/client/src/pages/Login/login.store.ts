import { combine, createEvent, createStore, sample } from "effector";
import { createEffect } from "effector/compat";
import {
  IdentityService,
  LoginCodeRequest,
} from "../../services/IdentityService";
import { userRequested } from "../../commons/stores/app.store";
import { ApiResponse } from "../../services/models/DataResponse";

const loginOneRequested = createEvent();
const loginTwoRequested = createEvent<LoginCodeRequest>();

const loginOneFx = createEffect(() => IdentityService.login());
const loginTwoFx = createEffect((props: LoginCodeRequest) =>
  IdentityService.loginCode(props)
);

sample({ clock: loginOneRequested, target: loginOneFx });
sample({ clock: loginTwoRequested, target: loginTwoFx });

const $loading = createStore<boolean>(false)
  .on(loginOneFx.pending, (_, p) => p)
  .on(loginTwoFx.pending, (_, p) => p);

const $step = createStore<number>(0)
  .on(loginOneFx.doneData, (_, p) => (p.success ? 1 : 0))
  .on(loginTwoFx.doneData, (_, p) => (p.success ? 2 : 1))
  .reset(loginOneRequested);

sample({
  clock: loginTwoFx.doneData,
  filter: (e: ApiResponse) => e.success,
  target: userRequested,
});

const $data = combine({
  loading: $loading,
  step: $step,
});

export { $data, loginOneRequested, loginTwoRequested };
