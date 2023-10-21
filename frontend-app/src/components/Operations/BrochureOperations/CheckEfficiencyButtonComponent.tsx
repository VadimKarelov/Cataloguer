import {Typography} from "antd";
import BaseButtonComponent from "../BaseButtonComponent";

/**
 * Компонент кнопки Проверить, открывающий свою модалку.
 */
const CheckEfficiencyButtonComponent = () => {
    /**
     * Свойства модалки.
     */
    const modalProps = {
        title: "Проверить каталог",
        okText: "Проверить",
        cancelText: "Отменить",
        children: (<Typography.Text>Вы действительно хотите проверить эффективность каталога?</Typography.Text>),
    };

    return (
        <BaseButtonComponent
            buttonProps={{buttonText: "Проверить"}}
            modalProps={modalProps}
        />
    );
};

export default CheckEfficiencyButtonComponent;