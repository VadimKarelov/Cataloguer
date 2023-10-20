import {Form, Result, Typography} from "antd";
import React from "react";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector, BrochureBaseInfoProps, customProp} from "../../types/BrochureTypes";
import "../../styles/Tabs/BrochureTab.css";
import {NO_DATA_TEXT} from "../../constants/Messages";

/**
 * Метаданные к разделу.
 */
const metaData: Readonly<BrochureBaseInfoProps[]> = [
    {name: "id", displayName: "Идентификатор", order: 1, isVisible: true},
    {name: "name", displayName: "Название", order: 2, isVisible: true},
    {name: "creationDate", displayName: "Дата выпуска каталога", order: 3, isVisible: true},
    {name: "edition", displayName: "Тираж", order: 4, isVisible: true},
    {name: "status", displayName: "Статус", order: 5, isVisible: true},
];

/**
 * Свойства компонента BrochureDescriptionComponent.
 */
interface BrochureDescriptionComponentProps extends BaseStoreInjector {
}

/**
 * Компонент описания каталога.
 */
const BrochureDescriptionComponent: React.FC<BrochureDescriptionComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const brochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Каталог, из которого достаём данные по свойству.
     */
    const parsableBrochure: customProp = brochure ?? {};

    /**
     * Отсортированные метаданные по порядку.
     */
    const sortedMetaData: Readonly<BrochureBaseInfoProps[]> = metaData
                                                                .slice(0)
                                                                .filter(md => md.isVisible)
                                                                .sort((m1, m2) => m1.order - m2.order);

    /**
     * Указатель на форму.
     */
    const [form] = Form.useForm();

    return (
        brochure !== null ?
        (
            <Form form={form} colon={false} layout={"vertical"} className={"brochure-form-style"}>
                {sortedMetaData.map(field => {
                    const value = parsableBrochure[field.name];
                    const label = (<div className={"brochure-form-item-label-style"}>{field.displayName}</div>);
                    return (
                        <Form.Item label={label}>
                            <Typography.Text className={"brochure-form-item-text-style"}>
                                {value ?? NO_DATA_TEXT}
                            </Typography.Text>
                        </Form.Item>
                    );
                })}
            </Form>
        ) : (
            <Result
                status="error"
                title={NO_DATA_TEXT}
            />
        )
    );
}));

export default BrochureDescriptionComponent;