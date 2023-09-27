import React from "react";
import {Result} from "antd";

/**
 * Компонент описания рассылки.
 */
const DistributionDescriptionComponent: React.FC = () => {
    return (
        <Result
            status="warning"
            title="Раздел 'Рассылка' находится в разработке."
            // extra={
            //     <Button type="primary" key="console">
            //         Go Console
            //     </Button>
            // }
        />
    );
};

export default DistributionDescriptionComponent;