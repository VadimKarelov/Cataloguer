import { notification } from 'antd';

/**
 * Тип, поволяющий менять иконку уведомления.
 */
type NotificationType = 'success' | 'info' | 'warning' | 'error';

/**
 * Функция, вызывающая уведомление.
 * @param title Заголовок.
 * @param message Текст сообщения.
 * @param messageType Тип иконки.
 */
export const openNotification = (title: string, message: string, messageType: NotificationType,) => {
    notification[messageType]({
        message: title,
        description: message,
        placement: 'bottomRight',
    });
};