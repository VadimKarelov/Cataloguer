import {Button, Popconfirm} from "antd";
import React from "react";
import {inject, observer} from "mobx-react";
import {openNotification} from "../../NotificationComponent";
import GoodsStore from "../../../stores/GoodsStore";
import {BaseStoreInjector, GoodsProps} from "../../../types/BrochureTypes";

/**
 * Свойства компонента DeleteBrochureButtonComponent.
 * @param goodsStore Хранилище товаров.
 * @param row Строка таблицы.
 */
interface DeleteGoodsButtonComponentProps extends BaseStoreInjector {
    goodsStore?: GoodsStore,
    row: GoodsProps,
}

/**
 * Компонент кнопки Удалить для товара.
 */
const DeleteGoodsButtonComponent: React.FC<DeleteGoodsButtonComponentProps> = inject("goodsStore", "brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const brochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Срабатывает при нажатии кнопки удалить.
     */
    const onOkClick = (): void => {
        const id = props.row.id,
                brochureId = brochure?.id ?? -1;

        const response = props.goodsStore?.handleDeleteBrochureGood(id, brochureId);

        response?.then(
            (resolve: string) => {openNotification("Успех", resolve, "success")},
            (error: string) => {openNotification("Ошибка", error, "error")}
        );
    };

    return (
        <Popconfirm title={`Удалить товар?`} okText={"Удалить"} cancelText={"Отменить"} onConfirm={onOkClick}>
            <Button danger>
                Удалить
            </Button>
        </Popconfirm>
    );
}));

export default DeleteGoodsButtonComponent;