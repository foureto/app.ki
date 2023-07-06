import { combine, createEvent, createStore, sample } from "effector";
import { createEffect } from "effector/compat";
import { IdentityService, LoginRequest } from "@services/IdentityService";
import { ApiResponse } from "@services/models/DataResponse";
import { userRequested } from "../../commons/stores/app.store";

const loginOneRequested = createEvent<LoginRequest>();

const loginOneFx = createEffect((props: LoginRequest) =>
  IdentityService.login(props)
);

sample({ clock: loginOneRequested, target: loginOneFx });

const $loading = createStore<boolean>(false).on(
  loginOneFx.pending,
  (_, p) => p
);

const $step = createStore<number>(0)
  .on(loginOneFx.doneData, (_, p) => (p.success ? 1 : 0))
  .reset(loginOneRequested);

sample({
  clock: loginOneFx.doneData,
  filter: (e: ApiResponse) => e.success,
  target: userRequested,
});

const $data = combine({
  loading: $loading,
  step: $step,
});

export { $data, loginOneRequested };
