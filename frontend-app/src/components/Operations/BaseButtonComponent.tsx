import {Button, Modal, Tooltip} from "antd";
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
    const [isModalOpen, setIsModalOpen] = useState(false);

    /**
     * Наведён ли курсор на кнопку открытия модалки.
     */
    const [isButtonHovered, setIsButtonHovered] = useState(false);

    /**
     * Срабатывает при нажатии на кнопку ок.
     * Пока только открывает модальное окно.
     */
    const onButtonClick = (): void => {
        setIsModalOpen(true);
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
        setIsModalOpen(false);
    };

    /**
     * Срабатывает при попытки нажать кнопку Ок.
     * Для создания каталога - кнопка сохранить.
     */
    const onFinish = (): void => {
        if (modalProps.onOkClick) {
            const shouldCloseHandler = modalProps.onOkClick();
            shouldCloseHandler.then(
                () => setIsModalOpen(false),
                () => setIsModalOpen(true),
            );
        }
    };

    return (
        <>
            <Modal
                open={isModalOpen}
                onCancel={onCancelClick}
                title={modalProps.title}
                okText={modalProps.okText}
                onOk={onFinish}
                cancelText={modalProps.cancelText}
            >
                {modalProps.children}
            </Modal>
            <Tooltip
                title={buttonProps.tooltip.title}
                open={buttonProps.isDisabled && isButtonHovered}
            >
                <Button
                    onMouseOver={e => setIsButtonHovered(true)}
                    onMouseLeave={e => setIsButtonHovered(false)}
                    disabled={buttonProps.isDisabled}
                    onClick={onButtonClick}
                >
                    {buttonProps.buttonText}
                </Button>
            </Tooltip>
        </>
    );
};

export default BaseButtonComponent;