import { Menu } from "antd";
import { MenuItemType } from "antd/es/menu/hooks/useItems";
import React from "react";
import { Link } from "react-router-dom";

const getItem = (label: string, link: string, children?: MenuItemType[]) => {
  return {
    key: label?.toLowerCase() ?? "_",
    label: <Link to={link}>{label}</Link>,
    children,
  };
};

const SideBar: React.FC = () => {
  const menu = React.useMemo(() => {
    return [
      getItem("Main", "/main"),
      getItem("Crypto", "/crypto/feed", [
        getItem("Feed", "/crypto/feed"),
        getItem("Arbitrage", "/"),
        getItem("Indicators", "/"),
      ]),
      getItem("Investments", "/"),
      getItem("Settings", "/"),
    ];
  }, []);

  return (
    <div className="sidebar">
      <div className="logo">Ari</div>
      <div className="side-menu">
        <Menu mode="inline" items={menu} />
      </div>
    </div>
  );
};

export default SideBar;
