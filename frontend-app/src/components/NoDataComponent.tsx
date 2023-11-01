import { Result } from "antd";
import React from "react";
import {NO_DATA_TEXT} from "../constants/Messages";

/**
 * Компонент с пустой страницей.
 */
const NoDataComponent: React.FC = () => (
    <Result
        status={"error"}
        title={NO_DATA_TEXT}
    />
);

export default NoDataComponent;