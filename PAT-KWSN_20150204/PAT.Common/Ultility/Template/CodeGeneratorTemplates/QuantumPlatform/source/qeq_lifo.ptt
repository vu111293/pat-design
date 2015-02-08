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
/// \brief QEQueue::postLIFO() implementation.

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

Q_DEFINE_THIS_MODULE(qeq_lifo)

//............................................................................
void QEQueue::postLIFO(QEvent const *e) {
    QF_CRIT_STAT_
    QF_CRIT_ENTRY_();

    QS_BEGIN_NOCRIT_(QS_QF_EQUEUE_POST_LIFO, QS::eqObj_, this)
        QS_TIME_();                                               // timestamp
        QS_SIG_(e->sig);                           // the signal of this event
        QS_OBJ_(this);                                    // this queue object
        QS_U8_(EVT_POOL_ID(e));                    // the pool Id of the event
        QS_U8_(EVT_REF_CTR(e));                  // the ref count of the event
        QS_EQC_(m_nFree);                            // number of free entries
        QS_EQC_(m_nMin);                         // min number of free entries
    QS_END_NOCRIT_()

    if (EVT_POOL_ID(e) != (uint8_t)0) {              // is it a dynamic event?
        EVT_INC_REF_CTR(e);                 // increment the reference counter
    }

    if (m_frontEvt != (QEvent *)0) {                // is the queue not empty?
               // the queue must be able to accept the event (cannot overflow)
        Q_ASSERT(m_nFree != (QEQueueCtr)0);

        ++m_tail;
        if (m_tail == m_end) {                       // need to wrap the tail?
            m_tail = (QEQueueCtr)0;                             // wrap around
        }

        m_ring[m_tail] = m_frontEvt;               // buffer the old front evt

        --m_nFree;                             // update number of free events
        if (m_nMin > m_nFree) {
            m_nMin = m_nFree;                         // update minimum so far
        }
    }

    m_frontEvt = e;                        // stick the new event to the front

    QF_CRIT_EXIT_();
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif
