import BaseButtonComponent from "../BaseButtonComponent";
import {Typography} from "antd";

/**
 * Компонент кнопки Удалить, открывающий свою модалку.
 */
const DeleteBrochureButtonComponent = () => {
    /**
     * Свойства модалки.
     */
    const modalProps = {
        title: "Удалить каталог",
        okText: "Удалить",
        cancelText: "Отменить",
        children: (<Typography.Text>Вы действительно хотите удалить каталог?</Typography.Text>),
    };

    return (
        <BaseButtonComponent
            buttonProps={{buttonText: "Удалить"}}
            modalProps={modalProps}
        />
    );
};

export default DeleteBrochureButtonComponent;