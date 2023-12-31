import {Form, Result, Typography} from "antd";
import React from "react";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector, BrochureBaseInfoProps, customProp} from "../../../types/BrochureTypes";
import "../../../styles/Tabs/BrochureTab.css";
import {NO_DATA_TEXT} from "../../../constants/Messages";
import dayjs from "dayjs";

/**
 * Метаданные к разделу.
 */
const metaData: Readonly<BrochureBaseInfoProps[]> = [
    {name: "id", displayName: "Идентификатор", order: 1, isVisible: true},
    {name: "name", displayName: "Название", order: 2, isVisible: true},
    {name: "date", displayName: "Дата выпуска каталога", order: 3, isVisible: true},
    {name: "edition", displayName: "Тираж", order: 4, isVisible: true},
    {name: "potentialIncome", displayName: "Эффективность", order: 5, isVisible: true},
    {name: "statusName", displayName: "Статус", order: 6, isVisible: true},
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
        brochure !== null ? (
            <Form id={"brochure_description"} form={form} colon={false} layout={"vertical"} className={"brochure-form-style"}>
                {sortedMetaData.map(field => {
                    const value = parsableBrochure[field.name];
                    const label = (<div className={"brochure-form-item-label-style"}>{field.displayName}</div>);
                    const valueToShow = field.name === "date" ? dayjs(value).format('DD.MM.YYYY HH:mm:ss') : value;
                    return (
                        <Form.Item key={`brochure_description_${field.name}`} label={label}>
                            <Typography.Text className={"brochure-form-item-text-style"}>
                                {valueToShow ?? NO_DATA_TEXT}
                            </Typography.Text>
                        </Form.Item>
                    );
                })}
            </Form>
        ) : (
            <Result
                status={"error"}
                title={NO_DATA_TEXT}
            />
        )
    );
}));

export default BrochureDescriptionComponent;