import BaseButtonComponent from "../BaseButtonComponent";
import {Typography} from "antd";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import React from "react";
import {inject, observer} from "mobx-react";

/**
 * Свойства компонента DeleteBrochureButtonComponent.
 */
interface DeleteBrochureButtonComponentProps extends BaseStoreInjector {
}

/**
 * Компонент кнопки Удалить, открывающий свою модалку.
 */
const DeleteBrochureButtonComponent: React.FC<DeleteBrochureButtonComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const currentBrochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Свойства модалки.
     */
    const modalProps = {
        title: "Удалить каталог",
        okText: "Удалить",
        cancelText: "Отменить",
        children: (<Typography.Text>Вы действительно хотите удалить каталог?</Typography.Text>),
    };

    /**
     * Свойства для кнопки, открывающей модальное окно.
     */
    const buttonProps = {
        buttonText: "Удалить",
        isDisabled: currentBrochure === null,
    };

    return (
        <BaseButtonComponent
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default DeleteBrochureButtonComponent;