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
     * Срабатывает при нажатии на кнопку.
     * Пока только открывает модальное окно.
     */
    const onButtonClick = () => {
        setIsOpen(true);
    };

    return (
        <>
            <Modal
                open={isOpen}
                onCancel={() => setIsOpen(false)}
                title={modalProps.title}
                okText={modalProps.okText}
                cancelText={modalProps.cancelText}
            >
                {modalProps.children}
            </Modal>
            <Button onClick={onButtonClick}>
                {buttonProps.buttonText}
            </Button>
        </>
    );
};

export default BaseButtonComponent;