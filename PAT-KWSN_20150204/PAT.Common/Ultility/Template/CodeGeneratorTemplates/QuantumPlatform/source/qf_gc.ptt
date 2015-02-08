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
/// \brief QF::gc() implementation.

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

Q_DEFINE_THIS_MODULE(qf_gc)

//............................................................................
void QF::gc(QEvent const *e) {
    if (EVT_POOL_ID(e) != (uint8_t)0) {              // is it a dynamic event?
        QF_CRIT_STAT_
        QF_CRIT_ENTRY_();

        if (EVT_REF_CTR(e) > (uint8_t)1) {   // isn't this the last reference?
            EVT_DEC_REF_CTR(e);                   // decrement the ref counter

            QS_BEGIN_NOCRIT_(QS_QF_GC_ATTEMPT, (void *)0, (void *)0)
                QS_TIME_();                                       // timestamp
                QS_SIG_(e->sig);                    // the signal of the event
                QS_U8_(EVT_POOL_ID(e));            // the pool Id of the event
                QS_U8_(EVT_REF_CTR(e));          // the ref count of the event
            QS_END_NOCRIT_()

            QF_CRIT_EXIT_();
        }
        else {         // this is the last reference to this event, recycle it
            uint8_t idx = (uint8_t)(EVT_POOL_ID(e) - 1);

            QS_BEGIN_NOCRIT_(QS_QF_GC, (void *)0, (void *)0)
                QS_TIME_();                                       // timestamp
                QS_SIG_(e->sig);                    // the signal of the event
                QS_U8_(EVT_POOL_ID(e));            // the pool Id of the event
                QS_U8_(EVT_REF_CTR(e));          // the ref count of the event
            QS_END_NOCRIT_()

            QF_CRIT_EXIT_();

            Q_ASSERT(idx < QF_maxPool_);

#ifdef Q_EVT_CTOR
            //lint -e1773                           Attempt to cast away const
            ((QEvent *)e)->~QEvent();     // call the xtor, cast 'const' away,
                             // which is legitimate, because it's a pool event
#endif
            //lint -e1773                           Attempt to cast away const
            QF_EPOOL_PUT_(QF_pool_[idx], (QEvent *)e);   // cast 'const' away,
                             // which is legitimate, because it's a pool event
        }
    }
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif

