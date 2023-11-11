import {Button, Modal} from "antd";
import React, {useState} from "react";
import {BaseButtonComponentProps} from "../../types/OperationsTypes";

/**
 * Базовый компонент для кнопок, открывающих модальные окна.
 */
const BaseButtonComponent: React.FC<BaseButtonComponentProps> = (props) => {
    /**
     * Свойства кнопки.
     */
    const buttonProps = props.buttonProps;

    /**
     * Свойства модального окна.
     */
    const modalProps = props.modalProps;

    /**
     * Открыто модальное окно или нет.
     */
    const [isOpen, setIsOpen] = useState(false);

    /**
     * Срабатывает при нажатии на кнопку ок.
     * Пока только открывает модальное окно.
     */
    const onButtonClick = (): void => {
        setIsOpen(true);
        const callback = props?.buttonProps?.onClick
        if (callback) {
            callback();
        }
    };

    /**
     * Срабатывает при нажатии на кнопку отменить/крестик.
     * Пока только открывает модальное окно.
     */
    const onCancelClick = (): void => {
        const callback = modalProps?.onCancelClick;
        if (callback) {
            callback();
        }
        setIsOpen(false);
    };

    /**
     * Срабатывает при попытки нажать кнопку Ок.
     * Для создания каталога - кнопка сохранить.
     */
    const onFinish = (): void => {
        if (modalProps.onOkClick) {
            const shouldCloseHandler = modalProps.onOkClick();
            shouldCloseHandler.then(
                () => setIsOpen(false),
                () => setIsOpen(true),
            );
        }
    };

    return (
        <>
            <Modal
                open={isOpen}
                onCancel={onCancelClick}
                title={modalProps.title}
                okText={modalProps.okText}
                onOk={onFinish}
                cancelText={modalProps.cancelText}
            >
                {modalProps.children}
            </Modal>
            <Button disabled={buttonProps.isDisabled} onClick={onButtonClick}>
                {buttonProps.buttonText}
            </Button>
        </>
    );
};

export default BaseButtonComponent;