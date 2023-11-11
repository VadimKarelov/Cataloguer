import {Button, Popconfirm} from "antd";
import React from "react";
import {inject, observer} from "mobx-react";
import {openNotification} from "../../NotificationComponent";
import {DistributionStore} from "../../../stores/DistributionStore";
import {EditDistributionDbProps} from "../../../types/DistributionTypes";

/**
 * Свойства компонента DeleteBrochureButtonComponent.
 * @param distributionStore Хранилище рассылок.
 * @param row Строка таблицы.
 */
interface DeleteDistributionButtonComponentProps {
    distributionStore?: DistributionStore,
    row: EditDistributionDbProps,
}

/**
 * Компонент кнопки Удалить для рассылки.
 */
const DeleteDistributionButtonComponent: React.FC<DeleteDistributionButtonComponentProps> = inject("distributionStore")(observer((props) => {
    /**
     * Срабатывает при нажатии кнопки удалить.
     */
    const onOkClick = (): void => {
        const {id, brochureId} = props.row;
        const response = props.distributionStore?.handleDeleteBrochureDistribution(id, brochureId);

        response?.then(
            (resolve: string) => {openNotification("Успех", resolve, "success")},
            (error: string) => {openNotification("Ошибка", error, "error")}
        );
    };

    return (
        <Popconfirm title={`Удалить рассылку?`} okText={"Удалить"} cancelText={"Отменить"} onConfirm={onOkClick}>
            <Button>
                Удалить
            </Button>
        </Popconfirm>
    );
}));

export default DeleteDistributionButtonComponent;