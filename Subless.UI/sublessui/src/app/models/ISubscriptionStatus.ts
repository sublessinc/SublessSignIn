export interface ISubscriptionStatus {
    isActive: boolean;
    isCancelled: boolean;
    billingDate: Date;
    cancellationDate: Date;
    subscriptionEndDate: Date;
}
