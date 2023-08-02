import React from "react";
import { Navigate } from "react-router-dom";
import { useStore } from "effector-react";
import { useFormik } from "formik";
import PageLoader from "@components/PageLoader";
import { $data, loginOneRequested } from "../../commons/stores/loginPage.store";
import { $userStore } from "../../commons/stores/app.store";

import "./login.scss";
import { LoginRequest } from "@services/IdentityService";

const LoginPage: React.FC = () => {
  const user = useStore($userStore);
  const { loading } = useStore($data);

  const formik = useFormik({
    initialValues: { login: "", password: "" },
    onSubmit: (values: LoginRequest) => {
      loginOneRequested(values);
    },
  });

  if (user) return <Navigate to={"/main"} />;
  if (loading) return <PageLoader />;

  return (
    <form onSubmit={formik.handleSubmit}>
      <div className="login-layout">
        <div className="login-container">
          <div className="login-form">
            <input
              type="text"
              name="login"
              onChange={formik.handleChange}
              value={formik.values.login}
              placeholder="Login"
            />
            <input
              type="password"
              name="password"
              onChange={formik.handleChange}
              value={formik.values.password}
              placeholder="Password"
            />
            <br />
            <button type="submit">login</button>
            <div className="text-small">terms of use goes here...</div>
          </div>
        </div>
      </div>
    </form>
  );
};

export default LoginPage;
