import BaseButtonComponent from "../BaseButtonComponent";
import {Typography} from "antd";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import React from "react";
import {inject, observer} from "mobx-react";
import {openNotification} from "../../NotificationComponent";

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
     * Срабатывает при нажатии кнопки удалить.
     */
    const onOkClick = (): Promise<void> => {
        const id = currentBrochure?.id ?? -1;
        const response = props.brochureStore?.handleDeleteBrochure(id);

        response?.then(
            (resolve: string) => {openNotification("Успех", resolve, "success")},
            (error: string) => {openNotification("Ошибка", error, "error")}
        );
        return Promise.resolve();
    };

    /**
     * Возвращает настройки хинта.
     */
    const getTooltipProps = () => {
        const title = "Необходимо выбрать каталог";
        return {title: title};
    };

    /**
     * Свойства модалки.
     */
    const modalProps = {
        title: "Удалить каталог",
        okText: "Удалить",
        cancelText: "Отменить",
        children: (<Typography.Text>{`Вы действительно хотите удалить каталог ${currentBrochure?.name}?`}</Typography.Text>),
        onOkClick: onOkClick,
    };

    /**
     * Свойства для кнопки, открывающей модальное окно.
     */
    const buttonProps = {
        buttonText: "Удалить",
        isDisabled: currentBrochure === null,
        tooltip: getTooltipProps(),
    };

    return (
        <BaseButtonComponent
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default DeleteBrochureButtonComponent;