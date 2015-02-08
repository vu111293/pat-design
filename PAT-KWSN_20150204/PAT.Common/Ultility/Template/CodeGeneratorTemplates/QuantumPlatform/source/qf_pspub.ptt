//////////////////////////////////////////////////////////////////////////////
// Product: QF/C++
// Last Updated for Version: 4.3.00
// Date of the Last Update:  Nov 01, 2011
//
//                    Q u a n t u m     L e a P s
//                    ---------------------------
//                    innovating embedded systems
//
// Copyright (C) 2002-2011 Quantum Leaps, LLC. All rights reserved.
//
// This software may be distributed and modified under the terms of the GNU
// General Public License version 2 (GPL) as published by the Free Software
// Foundation and appearing in the file GPL.TXT included in the packaging of
// this file. Please note that GPL Section 2[b] requires that all works based
// on this software must also be made publicly available under the terms of
// the GPL ("Copyleft").
//
// Alternatively, this software may be distributed and modified under the
// terms of Quantum Leaps commercial licenses, which expressly supersede
// the GPL and are specifically designed for licensees interested in
// retaining the proprietary status of their code.
//
// Contact information:
// Quantum Leaps Web site:  http://www.quantum-leaps.com
// e-mail:                  info@quantum-leaps.com
//////////////////////////////////////////////////////////////////////////////
#include "qf_pkg.h"
#include "qassert.h"

/// \file
/// \ingroup qf
/// \brief QF::publish() implementation.

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

Q_DEFINE_THIS_MODULE(qf_pspub)

//............................................................................
#ifndef Q_SPY
void QF::publish(QEvent const *e) {
#else
void QF::publish(QEvent const *e, void const *sender) {
#endif
         // make sure that the published signal is within the configured range
    Q_REQUIRE(e->sig < QF_maxSignal_);

    QF_CRIT_STAT_
    QF_CRIT_ENTRY_();

    QS_BEGIN_NOCRIT_(QS_QF_PUBLISH, (void *)0, (void *)0)
        QS_TIME_();                                           // the timestamp
        QS_OBJ_(sender);                                  // the sender object
        QS_SIG_(e->sig);                            // the signal of the event
        QS_U8_(EVT_POOL_ID(e));                    // the pool Id of the event
        QS_U8_(EVT_REF_CTR(e));                  // the ref count of the event
    QS_END_NOCRIT_()

    if (EVT_POOL_ID(e) != (uint8_t)0) {              // is it a dynamic event?
        EVT_INC_REF_CTR(e);         // increment the reference counter, NOTE01
    }
    QF_CRIT_EXIT_();

#if (QF_MAX_ACTIVE <= 8)
    uint8_t tmp = QF_subscrList_[e->sig].m_bits[0];
    while (tmp != (uint8_t)0) {
        uint8_t p = Q_ROM_BYTE(QF_log2Lkup[tmp]);
        tmp &= Q_ROM_BYTE(QF_invPwr2Lkup[p]);      // clear the subscriber bit
        Q_ASSERT(active_[p] != (QActive *)0);            // must be registered

                           // POST() asserts internally if the queue overflows
        active_[p]->POST(e, sender);
    }
#else
    uint8_t i = Q_DIM(QF_subscrList_[0].m_bits);// number of bytes in the list
    do {                       // go through all bytes in the subsciption list
        --i;
        uint8_t tmp = QF_subscrList_[e->sig].m_bits[i];
        while (tmp != (uint8_t)0) {
            uint8_t p = Q_ROM_BYTE(QF_log2Lkup[tmp]);
            tmp &= Q_ROM_BYTE(QF_invPwr2Lkup[p]);  // clear the subscriber bit
            p = (uint8_t)(p + (i << 3));                // adjust the priority
            Q_ASSERT(active_[p] != (QActive *)0);        // must be registered

                           // POST() asserts internally if the queue overflows
            active_[p]->POST(e, sender);
        }
    } while (i != (uint8_t)0);
#endif

    gc(e);                            // run the garbage collector, see NOTE01
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif

//////////////////////////////////////////////////////////////////////////////
// NOTE01:
// QF::publish() increments the reference counter to prevent premature
// recycling of the event while the multicasting is still in progress.
// At the end of the function, the garbage collector step decrements the
// reference counter and recycles the event if the counter drops to zero.
// This covers the case when the event was published without any subscribers.
//
