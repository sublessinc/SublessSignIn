import { ISubscriptionStatus } from "./ISubscriptionStatus";

export class SubscriptionStatus implements ISubscriptionStatus {
    constructor(
        public isActive: boolean,
        public isCancelled: boolean,
        public billingDate: Date,
        public cancellationDate: Date,
        public subscriptionEndDate: Date,
    ) { }

}