# CONTEXT.md

# Amplifika Crowdfunding Platform

## Overview

Amplifika is an independent vinyl record label that needs a lightweight crowdfunding platform to finance vinyl pressings, album pre-orders, limited editions, and other music-related projects.

This project is intentionally designed as a focused MVP rather than a generic crowdfunding marketplace.

Unlike platforms such as Kickstarter or Catarse, only Amplifika administrators can create campaigns.

Supporters can:

* Browse active campaigns
* Select a reward
* Fill in their personal information
* Pay via Pix
* Receive campaign rewards

The system must be simple, maintainable, and easy to evolve.

---

# Product Goal

Create a crowdfunding platform that allows Amplifika to:

* Publish campaigns
* Receive supporter contributions
* Manage rewards
* Receive Pix payments
* Track deliveries
* Manage campaigns through an admin panel

The platform's primary purpose is validating vinyl-related crowdfunding campaigns with minimal operational complexity.

---

# MVP Scope

## Public Area

Visitors can:

* View published campaigns
* View campaign details
* View funding goal
* View amount raised
* View campaign deadline
* View available rewards
* Select a reward
* Submit support information
* Receive a Pix payment request

## Administrative Area

Administrators can:

* Create campaigns
* Edit campaigns
* Publish campaigns
* Close campaigns
* Create rewards
* View supporters
* View payments
* Manage deliveries
* Export supporter data as CSV

---

# Out of Scope

The following features are intentionally excluded from the MVP:

* Public creator registration
* Campaign marketplace
* Credit card payments
* Boleto payments
* Public user accounts
* Supporter login
* Comments
* Chat
* Mobile application
* Automatic refunds
* Multi-vendor support

---

# Core Domain Concepts

## Campaign

Represents a crowdfunding project.

Attributes:

* id
* title
* slug
* description
* goalAmount
* currentAmount
* startsAt
* endsAt
* status
* coverImage

Statuses:

* Draft
* Published
* Closed
* Cancelled

Business Rules:

* Draft campaigns can be edited
* Only Published campaigns are visible publicly
* Closed campaigns cannot receive new pledges
* Raised amount only considers paid pledges

---

## Reward

Represents a campaign reward.

Attributes:

* id
* campaignId
* title
* description
* minimumAmount
* quantityLimit
* requiresShipping
* estimatedDelivery

Business Rules:

* Belongs to a Campaign
* Can require shipping
* Can have limited quantity
* Cannot be selected when unavailable

---

## Supporter

Represents a contributor.

Attributes:

* id
* name
* email
* phone
* document

---

## Pledge

Represents a support intention.

Attributes:

* id
* campaignId
* rewardId
* supporterId
* amount
* status
* createdAt
* paidAt

Statuses:

* AwaitingPix
* Paid
* Expired
* Cancelled

Important:

A pledge is NOT a payment.

A pledge becomes valid only after Pix confirmation.

---

## Pix Payment

Represents a Pix charge generated for a pledge.

Attributes:

* id
* pledgeId
* providerPaymentId
* qrCode
* copyPasteCode
* status
* expiresAt
* paidAt

Statuses:

* Pending
* Paid
* Expired
* Cancelled

---

## Delivery

Represents reward fulfillment.

Attributes:

* id
* pledgeId
* status
* trackingCode

Statuses:

* NotStarted
* Preparing
* Shipped
* Delivered
* Problem

Business Rules:

* Only paid pledges generate deliveries

---

# Ubiquitous Language

Use these terms consistently across the entire codebase:

| Business Term | Meaning                      |
| ------------- | ---------------------------- |
| Campaign      | Crowdfunding project         |
| Reward        | Supporter benefit            |
| Supporter     | Person supporting a campaign |
| Pledge        | Support intention            |
| Pix Payment   | Payment request              |
| Delivery      | Reward fulfillment           |
| Goal Amount   | Campaign target              |
| Raised Amount | Sum of paid pledges          |

Avoid synonyms.

Example:

Use "Pledge" everywhere instead of:

* Contribution
* Donation
* Funding
* Order

---

# Bounded Contexts

## Campaigns Context

Responsibilities:

* Campaign lifecycle
* Campaign publication
* Campaign visibility

---

## Rewards Context

Responsibilities:

* Reward management
* Reward availability

---

## Pledges Context

Responsibilities:

* Support registration
* Supporter information
* Address management

---

## Pix Payments Context

Responsibilities:

* Pix generation
* Pix confirmation
* Webhook processing

---

## Deliveries Context

Responsibilities:

* Reward shipment tracking

---

## Administration Context

Responsibilities:

* Internal management
* Auditing
* CSV exports

---

# Critical Business Flows

## Campaign Funding Flow

1. Admin publishes campaign
2. Visitor opens campaign page
3. Visitor selects reward
4. Visitor submits support information
5. System creates pledge
6. System generates Pix charge
7. Visitor pays Pix
8. Pix provider confirms payment
9. Pledge becomes Paid
10. Campaign raised amount is updated
11. Delivery is created if required

---

## Reward Reservation Flow

Reward reservation only becomes valid after payment confirmation.

Unpaid pledges must not increase campaign funding.

---

## Delivery Flow

1. Pledge becomes Paid
2. Delivery record is created
3. Admin updates shipment status
4. Tracking information is stored

---

# Architectural Principles

## Style

Modular Monolith

The system should be built as a modular monolith with clear domain boundaries.

Do not prematurely introduce microservices.

---

## Domain-Driven Design

Prefer:

* Entities
* Value Objects
* Aggregates
* Repositories
* Domain Services
* Factories

Keep business rules inside the domain.

Avoid business logic in controllers.

---

## Anti-Corruption Layer

Pix providers must never leak provider-specific concepts into the domain.

Create adapters such as:

* PixProviderGateway
* PixWebhookTranslator
* PixProviderAdapter

External statuses must be translated into internal statuses.

Example:

Provider Status → Domain Status

CONCLUIDA → Paid

ATIVA → Pending

REMOVIDA → Cancelled

---

# Technical Recommendations

Frontend:

* Next.js
* React
* TypeScript
* TailwindCSS

Backend:

* ASP.NET Core
* Clean Architecture
* DDD

Database:

* PostgreSQL

Authentication:

* Admin area only

Payments:

* Pix Provider Adapter

Deployment:

* Docker

---

# Future Roadmap

Potential future features:

* Credit card payments
* Public supporter accounts
* Campaign updates
* Automatic refunds
* Marketplace model
* Multiple creators
* Mobile application
* Analytics dashboard

These features must not influence MVP complexity today.

---

# Success Criteria

The MVP is successful when Amplifika can:

* Publish campaigns
* Receive Pix payments
* Track supporters
* Manage rewards
* Manage deliveries
* Export supporter data

without relying on external crowdfunding platforms.
